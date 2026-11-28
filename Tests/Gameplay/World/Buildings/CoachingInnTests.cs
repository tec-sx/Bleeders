using System.Numerics;

namespace Bleeders.Gameplay.Tests.World.Buildings;

/// <summary>The coaching inn plays as it was made in the building creator: doors open, a lock waits for its key, stairs climb and the elevator answers its call.</summary>
public sealed class CoachingInnTests : IDisposable
{
    private const float Ground = 0.2f;

    private const float FirstFloor = 3.2f;

    private const float SecondFloor = 6.2f;

    private static readonly Vector3 Street = CoachingInnTestsBuilder.OnPlan(-1.5f, Ground, -6.5f);

    private static readonly Vector3 Hall = CoachingInnTestsBuilder.OnPlan(-1.5f, Ground, -2);

    private static readonly Vector3 IntoTheInn = CoachingInnTestsBuilder.PlanWay(0, -0.3f, 1);

    private static readonly Vector3 TowardTheStore = CoachingInnTestsBuilder.PlanWay(1, -0.3f, 0);

    private readonly CoachingInnTestsBuilder _builder = new();

    public CoachingInnTests()
    {
        _builder.Start();
    }

    public void Dispose() => _builder.Dispose();

    [Fact]
    public void TheFoxOpensTheFrontDoorAndWalksIntoTheHall()
    {
        bool outside = _builder.WalkTo(Street, 60);
        bool opened = _builder.OpenDoor(CoachingInnTestsBuilder.FrontDoor, IntoTheInn, 3);
        bool inside = _builder.WalkTo(Hall);

        outside.Should().BeTrue();
        opened.Should().BeTrue();
        inside.Should().BeTrue("the open front door lets it into the hall");
    }

    [Fact]
    public void TheStoreDoorStaysLockedWithoutTheCellarKey()
    {
        EnterTheHall();

        _builder.OpenDoor(CoachingInnTestsBuilder.StoreDoor, TowardTheStore, 1.5f, 6);

        _builder.IsOpen(CoachingInnTestsBuilder.StoreDoor).Should().BeFalse("the store door is locked with the cellar key");
    }

    [Fact]
    public void TheCellarKeyOpensTheStoreDoor()
    {
        Vector3 store = CoachingInnTestsBuilder.OnPlan(3.5f, Ground, -0.5f);
        bool tookKey = _builder.TakeTheKey();
        EnterTheHall();

        bool opened = _builder.OpenDoor(CoachingInnTestsBuilder.StoreDoor, TowardTheStore, 1.5f);
        bool inTheStore = _builder.WalkTo(store);

        tookKey.Should().BeTrue();
        opened.Should().BeTrue("the fox carries the cellar key");
        inTheStore.Should().BeTrue();
    }

    [Fact]
    public void TheFoxClimbsTheStairsCallsTheElevatorAndRidesItToTheTopStorey()
    {
        Vector3 landing = CoachingInnTestsBuilder.OnPlan(1.1f, FirstFloor, 3.5f);
        Vector3 upperStore = CoachingInnTestsBuilder.OnPlan(3.5f, FirstFloor, -1);
        Vector3 towardTheShaft = CoachingInnTestsBuilder.PlanWay(0, -0.3f, -1);
        Vector3 down = CoachingInnTestsBuilder.PlanWay(0, -1, 0.3f);
        Vector3 top = CoachingInnTestsBuilder.OnPlan(0, SecondFloor, 0);
        EnterTheHall();

        bool upstairs = _builder.WalkTo(landing);
        bool inTheStore = _builder.WalkTo(upperStore);
        bool called = _builder.Click("elevator.1.call.1", towardTheShaft, 1.2f, () => _builder.IsPlatformAt(1), 20, above: 0.09f);
        bool rode = _builder.Click(CoachingInnTestsBuilder.Platform, down, 2, () => _builder.FoxPlace.Y > top.Y - 0.15f, 20);

        upstairs.Should().BeTrue("the kitchen's stairs climb to the first floor");
        inTheStore.Should().BeTrue();
        called.Should().BeTrue("the call button on the first floor brings the platform up from the cellar");
        rode.Should().BeTrue("the called platform carries the fox up a stop to the second floor");
    }

    private void EnterTheHall()
    {
        _builder.WalkTo(Street, 60);
        _builder.OpenDoor(CoachingInnTestsBuilder.FrontDoor, IntoTheInn, 3);
        _builder.WalkTo(Hall);
    }
}
