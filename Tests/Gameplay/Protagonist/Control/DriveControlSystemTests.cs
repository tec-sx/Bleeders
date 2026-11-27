using System.Numerics;

using Arch.Core;

using Bleeders.Gameplay.Protagonist.Control;

using FlickerVoid.Engine.Physics;
using FlickerVoid.Engine.Rendering.Extraction;
using FlickerVoid.Engine.Runtime.Components;
using FlickerVoid.Engine.Vehicles;

using Silk.NET.Input;

namespace Bleeders.Gameplay.Tests.Protagonist.Control;

using World = Arch.Core.World;

public sealed class DriveControlSystemTests : IDisposable
{
    private readonly DriveControlSystemTestsBuilder _builder = new();

    private World World => _builder.Game.World;

    public void Dispose() => _builder.Dispose();

    [Fact]
    public void EBesideAVehiclePutsTheFoxInItUnseen()
    {
        _builder.Tap(Key.E);

        World.Get<Driving>(_builder.Fox).Vehicle.Should().Be(_builder.Car);
        World.Get<DrivenVehicle>(_builder.Car).Driver.Should().Be(_builder.Fox);
        World.Has<Hidden>(_builder.Fox).Should().BeTrue();
        World.Has<WheeledBody>(_builder.Car).Should().BeTrue();
    }

    [Fact]
    public void EFarFromAnyVehicleDoesNothing()
    {
        World.Get<Transform>(_builder.Fox).Position = new Vector3(8, 0, 0);

        _builder.Tap(Key.E);

        World.Has<Driving>(_builder.Fox).Should().BeFalse();
        World.Has<DrivenVehicle>(_builder.Car).Should().BeFalse();
    }

    [Fact]
    public void WDrivesTheVehicleForwardWithTheFoxInIt()
    {
        _builder.Tap(Key.E);
        _builder.Run(1);

        _builder.Hold(Key.W);
        _builder.Run(2);

        Vector3 car = World.Get<Transform>(_builder.Car).Position;
        Vector3 fox = World.Get<Transform>(_builder.Fox).Position;
        car.Z.Should().BeGreaterThan(1);
        float apart = Vector2.Distance(new Vector2(car.X, car.Z), new Vector2(fox.X, fox.Z));
        apart.Should().BeLessThan(0.5f, "the fox rides in the car instead of walking");
    }

    [Fact]
    public void DSteersTheVehicleToItsRight()
    {
        _builder.Tap(Key.E);
        _builder.Run(1);

        _builder.Hold(Key.W);
        _builder.Hold(Key.D);
        _builder.Run(3);

        World.Get<Transform>(_builder.Car).Position.X.Should().BeLessThan(-0.5f, "a vehicle's left is +X");
    }

    [Fact]
    public void SAgainstTheWayItRollsBrakesBeforeItReverses()
    {
        _builder.Tap(Key.E);
        _builder.Run(1);
        _builder.Hold(Key.W);
        _builder.Run(2);
        _builder.Release(Key.W);

        _builder.Hold(Key.S);
        _builder.Run(0.1f);

        DriveInput input = World.Get<DriveInput>(_builder.Car);
        input.Brake.Should().Be(1);
        input.Forward.Should().Be(0);
        _builder.Run(6);
        World.Get<DriveInput>(_builder.Car).Forward.Should().Be(-1, "once stopped, the same key reverses");
    }

    [Fact]
    public void SpaceHoldsTheHandBrake()
    {
        _builder.Tap(Key.E);

        _builder.Hold(Key.Space);
        _builder.Run(0.1f);

        World.Get<DriveInput>(_builder.Car).HandBrake.Should().Be(1);
    }

    [Fact]
    public void EAgainLetsTheFoxOutOnTheLeftAndTheVehicleStands()
    {
        _builder.Tap(Key.E);
        _builder.Run(1);
        _builder.Hold(Key.W);
        _builder.Run(1);
        _builder.Release(Key.W);

        _builder.Tap(Key.E);
        Vector3 car = World.Get<Transform>(_builder.Car).Position;
        Vector3 fox = World.Get<Transform>(_builder.Fox).Position;
        _builder.Run(5);

        World.Has<Driving>(_builder.Fox).Should().BeFalse();
        World.Has<Hidden>(_builder.Fox).Should().BeFalse();
        (fox.X - car.X).Should().BeGreaterThan(1, "it stands clear of the car's left side");
        World.Has<DrivenVehicle>(_builder.Car).Should().BeFalse();
        World.Has<WheeledBody>(_builder.Car).Should().BeFalse("the car stopped and stands again");
        Vector3 stood = World.Get<Transform>(_builder.Fox).Position;
        _builder.Hold(Key.W);
        _builder.Run(1);
        Vector3 walked = World.Get<Transform>(_builder.Fox).Position;
        Vector3.Distance(stood, walked).Should().BeGreaterThan(0.5f, "the keys walk the fox again");
    }
}
