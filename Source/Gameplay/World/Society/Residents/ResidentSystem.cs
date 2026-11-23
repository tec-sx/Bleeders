using System.Numerics;

using Arch.Core;
using Arch.System;

using FlickerVoid.Engine.Ai.Paths;
using FlickerVoid.Engine.Ai.Senses;
using FlickerVoid.Engine.Characters.Movement;
using FlickerVoid.Engine.Dialogue.Conversations;
using FlickerVoid.Engine.Runtime.Components;
using FlickerVoid.Engine.Runtime.Events;
using FlickerVoid.Engine.Runtime.Loop;

namespace Bleeders.Gameplay.World.Society.Residents;

using World = Arch.Core.World;

/// <summary>
/// Runs residents each fixed step, as the Unreal AI's perception and wandering did: one talking faces whom it talks to, one that sees
/// someone stops and watches them until it loses sight of them, one that hears a noise turns toward it, and otherwise it walks to a
/// random point of the ground within its wander radius of home after resting.
/// </summary>
public sealed class ResidentSystem : BaseSystem<World, GameTime>
{
    private readonly EventBus _events;
    private readonly PathSystem _paths;
    private readonly PerceptionSystem _perception;
    private readonly DialogueSystem _dialogue;
    private readonly Random _random;

    public ResidentSystem(Game game, PathSystem paths, PerceptionSystem perception, DialogueSystem dialogue, Random random)
        : base(game.World)
    {
        _events = game.Events;
        _paths = paths;
        _perception = perception;
        _dialogue = dialogue;
        _random = random;
        
        ArrayRegistry.Add<ResidentState>();
        
        game.Lifecycle.OnActivate((Entity entity, ref Resident resident) => Activate(entity, resident));
        game.Lifecycle.OnDeactivate((Entity entity, ref Resident _) => Deactivate(entity));

        _events.Handle((in TargetSeenEvent seen) => Watch(seen.Perceiver, seen.Target));
        _events.Handle((in TargetLostEvent lost) => Lose(lost.Perceiver, lost.Target));
        _events.Handle((in NoiseHeardEvent heard) => Hear(heard.Perceiver, heard.Point));
    }

    public override void Update(in GameTime t)
    {
        float seconds = (float)t.Elapsed.TotalSeconds;
        QueryDescription dwellers = Dweller.Query;
        World.Query(in dwellers, (Entity entity) =>
        {
            Dweller dweller = Dweller.From(World, entity);
            ref ResidentState state = ref dweller.State;
            Entity faced = Entity.Null;
            if (_dialogue.IsInConversation && _dialogue.Owner == entity)
            {
                faced = _dialogue.Instigator;
            }
            else if (IsWatching(state))
            {
                faced = state.Watching;
            }

            if (faced != Entity.Null && World.IsAlive(faced) && World.Has<Transform>(faced))
            {
                Vector3 point = World.Get<Transform>(faced).Position;
                _events.Send(new FaceCommand(entity, point));
                state.RestLeft = dweller.Resident.RestSeconds;
                return;
            }

            Wander(dweller, seconds);
        });
    }

    private void Wander(Dweller dweller, float seconds)
    {
        Entity entity = dweller.Entity;
        ref Resident resident = ref dweller.Resident;
        ref ResidentState state = ref dweller.State;
        if (resident.WanderRadius <= 0 || World.Has<Destination>(entity) || World.Has<Facing>(entity))
        {
            return;
        }

        state.RestLeft -= seconds;
        if (state.RestLeft > 0)
        {
            return;
        }

        state.RestLeft = resident.RestSeconds;
        if (_paths.TryFindRandomPoint(state.Home, resident.WanderRadius, _random, out Vector3 point))
        {
            _events.Send(new MoveCommand(entity, point));
        }
    }

    private void Watch(Entity resident, Entity target)
    {
        if (Dweller.TryGet(World, resident, out Dweller dweller) && !IsWatching(dweller.State))
        {
            dweller.State.Watching = target;
        }
    }

    private void Lose(Entity resident, Entity target)
    {
        if (!Dweller.TryGet(World, resident, out Dweller dweller))
        {
            return;
        }

        ref ResidentState state = ref dweller.State;
        if (state.Watching != target)
        {
            return;
        }

        state.Watching = Entity.Null;
        IReadOnlyList<Entity> seen = _perception.GetSeen(resident);
        if (seen.Count > 0)
        {
            state.Watching = seen[0];
        }
    }

    private void Hear(Entity resident, Vector3 point)
    {
        if (!Dweller.TryGet(World, resident, out Dweller dweller))
        {
            return;
        }

        ref ResidentState state = ref dweller.State;
        if (!IsWatching(state))
        {
            _events.Send(new FaceCommand(resident, point));
            state.RestLeft = dweller.Resident.RestSeconds;
        }
    }

    private bool IsWatching(in ResidentState state) => state.Watching != Entity.Null && World.IsAlive(state.Watching);

    private void Activate(Entity entity, Resident resident)
    {
        World.TryGet(entity, out Transform transform);
        ResidentState state = new() { Home = transform.Position, RestLeft = resident.RestSeconds, Watching = Entity.Null };
        if (World.Has<ResidentState>(entity))
        {
            World.Set(entity, state);
        }
        else
        {
            World.Add(entity, state);
        }
    }

    private void Deactivate(Entity entity)
    {
        if (World.Has<ResidentState>(entity))
        {
            World.Remove<ResidentState>(entity);
        }
    }
}
