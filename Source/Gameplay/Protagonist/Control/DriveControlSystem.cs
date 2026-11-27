using System.Numerics;

using Arch.Core;
using Arch.System;

using FlickerVoid.Engine.Characters.Movement;
using FlickerVoid.Engine.Dialogue.Conversations;
using FlickerVoid.Engine.Gameplay.Tags;
using FlickerVoid.Engine.Physics;
using FlickerVoid.Engine.Platform.Input;
using FlickerVoid.Engine.Rendering.Extraction;
using FlickerVoid.Engine.Runtime.Components;
using FlickerVoid.Engine.Runtime.Entities;
using FlickerVoid.Engine.Runtime.Events;
using FlickerVoid.Engine.Runtime.Loop;
using FlickerVoid.Engine.Vehicles;

namespace Bleeders.Gameplay.Protagonist.Control;

using World = Arch.Core.World;

/// <summary>
/// Driving: a press of <see cref="Drive"/> (E) beside a vehicle puts the protagonist in it, and another lets it out on the vehicle's
/// left. While it drives, <see cref="Move"/> (W, A, S and D) is the accelerator and the steering, relative to the vehicle, pressing
/// against the way it rolls brakes before it reverses, and <see cref="HandBrake"/> (Space) holds the hand brake.
/// </summary>
public sealed class DriveControlSystem : BaseSystem<World, GameTime>
{
    public static readonly Tag Drive = new("Input.Drive");

    public static readonly Tag HandBrake = new("Input.HandBrake");

    public static readonly Tag Move = WalkControlSystem.Move;

    /// <summary>How far from a vehicle's middle, in metres, the protagonist can get in.</summary>
    public const float EnterRange = 3.5f;

    /// <summary>The speed, in metres a second, above which pressing against the way the vehicle rolls brakes instead of driving.</summary>
    private const float RollingSpeed = 1;

    /// <summary>How far beside the vehicle's outermost wheel the protagonist gets out, in metres.</summary>
    private const float LeaveGap = 0.9f;

    private static readonly QueryDescription Vehicles = new QueryDescription().WithAll<Vehicle, Transform>().WithNone<DrivenVehicle>();

    private readonly EventBus _events;
    private readonly InputActions _actions;
    private readonly IPhysicsSystem _physics;
    private readonly DialogueSystem _dialogue;
    private readonly VehicleDriveSystem? _vehicles;

    public DriveControlSystem(World world, EventBus events, InputActions actions, IPhysicsSystem physics, DialogueSystem dialogue, VehicleDriveSystem? vehicles)
        : base(world)
    {
        _events = events;
        _actions = actions;
        _physics = physics;
        _dialogue = dialogue;
        _vehicles = vehicles;
        ArrayRegistry.Add<Driving>();
        ArrayRegistry.Add<Hidden>();
        events.Handle((in InputActionEvent action) => HandleInput(action));
    }

    public override void Update(in GameTime t)
    {
        if (!Player.TryFind(World, out Player player) || !World.TryGet(player.Entity, out Driving driving))
        {
            return;
        }

        Entity vehicle = driving.Vehicle;
        if (!World.IsAlive(vehicle) || !World.Has<DriveInput>(vehicle))
        {
            return;
        }

        Vector2 value = _actions.GetValue(Move);
        DriveInput input = new() { Forward = value.Y, Right = value.X };
        Quaternion rotation = World.Get<Transform>(vehicle).Rotation;
        Vector3 facing = Vector3.Transform(Vector3.UnitZ, rotation);
        Vector3 velocity = _physics.GetVelocity(vehicle);
        float speed = Vector3.Dot(velocity, facing);
        if (input.Forward * speed < 0 && MathF.Abs(speed) > RollingSpeed)
        {
            input.Forward = 0;
            input.Brake = 1;
        }

        if (_actions.IsDown(HandBrake))
        {
            input.HandBrake = 1;
        }

        World.Set(vehicle, input);
    }

    private void HandleInput(in InputActionEvent action)
    {
        if (action.Action != Drive || action.Phase != InputPhase.Pressed || _dialogue.IsInConversation)
        {
            return;
        }

        if (!Player.TryFind(World, out Player player))
        {
            return;
        }

        Entity protagonist = player.Entity;
        if (World.TryGet(protagonist, out Driving driving))
        {
            Leave(protagonist, driving.Vehicle);
            return;
        }

        Entity vehicle = FindNearest(player.Transform.Position);
        if (vehicle != Entity.Null)
        {
            Enter(protagonist, vehicle);
        }
    }

    /// <summary>The nearest vehicle that takes a driver and has none within <see cref="EnterRange"/> of the position, or <see cref="Entity.Null"/>.</summary>
    private Entity FindNearest(Vector3 position)
    {
        Entity nearest = Entity.Null;
        if (_vehicles is not { } vehicles)
        {
            return nearest;
        }

        float nearestDistance = EnterRange;
        World.Query(in Vehicles, (Entity entity, ref Transform transform) =>
        {
            float distance = Vector3.Distance(transform.Position, position);
            if (distance <= nearestDistance && vehicles.CanDrive(entity))
            {
                nearest = entity;
                nearestDistance = distance;
            }
        });
        return nearest;
    }

    /// <summary>Seats the protagonist in the vehicle, unseen, so it and the camera on it ride along, and hands the vehicle to it.</summary>
    private void Enter(Entity protagonist, Entity vehicle)
    {
        _events.Send(new StopCommand(protagonist));
        Transform seat = World.Get<Transform>(protagonist);
        seat.Position = Vector3.Zero;
        seat.Rotation = Quaternion.Identity;
        TransformHierarchy.Attach(World, protagonist, vehicle, seat);
        World.Add(protagonist, new Driving { Vehicle = vehicle }, default(Hidden));
        World.Add(vehicle, new DrivenVehicle { Driver = protagonist });
    }

    /// <summary>Stands the protagonist on the vehicle's left, facing as it does, and leaves the vehicle to brake and stand.</summary>
    private void Leave(Entity protagonist, Entity vehicle)
    {
        World.Remove<Driving, Hidden, NestedUnder>(protagonist);
        if (!World.IsAlive(vehicle))
        {
            return;
        }

        Transform place = World.Get<Transform>(vehicle);
        float side = LeaveGap;
        if (World.TryGet(vehicle, out WheeledBody body))
        {
            foreach (BodyWheel wheel in body.Wheels)
            {
                side = MathF.Max(side, wheel.Position.X + LeaveGap);
            }
        }

        Vector3 left = Vector3.Transform(Vector3.UnitX, place.Rotation);
        left.Y = 0;
        ref Transform stood = ref World.Get<Transform>(protagonist);
        stood.Position = place.Position + (left * side);
        stood.Rotation = place.Rotation;
        if (World.Has<DrivenVehicle>(vehicle))
        {
            World.Remove<DrivenVehicle>(vehicle);
        }
    }
}
