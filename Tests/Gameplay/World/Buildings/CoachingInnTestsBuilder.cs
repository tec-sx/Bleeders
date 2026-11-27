using System.Numerics;

using Arch.Core;

using Bleeders.Gameplay.Tests.Story;

using FlickerVoid.Engine.Buildings;
using FlickerVoid.Engine.Creations;
using FlickerVoid.Engine.Interaction.Interactors;
using FlickerVoid.Engine.Interaction.Toggles;
using FlickerVoid.Engine.Inventory.Inventories;
using FlickerVoid.Engine.Gameplay.Tags;

namespace Bleeders.Gameplay.Tests.World.Buildings;

/// <summary>
/// Plays the main scene as <see cref="StreetTestsBuilder"/> does around the coaching inn at the street's east end, a building made in
/// the editor's building creator (<c>Buildings/Inn.fvbuilding</c>), and clicks its doors, buttons and platform as the player does.
/// </summary>
internal sealed class CoachingInnTestsBuilder : IDisposable
{
    public const string FrontDoor = "storey.0.door.19";

    public const string StoreDoor = "storey.0.door.26";

    public const string Platform = "elevator.1";

    private static readonly Tag CellarKey = new("Key.Cellar");

    /// <summary>Where the inn stands, sunk so its ground floor is level with the street and turned half round so its front faces it.</summary>
    private static readonly Vector3 Origin = new(24, -0.32f, 0);

    public StreetTestsBuilder Street { get; } = new();

    public StoryLoopTestsBuilder Story => Street.Story;

    public Entity Fox => Story.Fox;

    public CoachingInnTestsBuilder Start()
    {
        Street.Start();
        return this;
    }

    /// <summary>The point in the world at a place on the inn's plan (as the building creator shows it) and a height above its origin.</summary>
    public static Vector3 OnPlan(float x, float y, float z) => Origin + new Vector3(-x, y, -z);

    /// <summary>A direction on the inn's plan turned into the world's.</summary>
    public static Vector3 PlanWay(float x, float y, float z) => new(-x, y, -z);

    public Entity Part(string id)
    {
        Entity inn = Story.Find("Coaching Inn");
        CreationParts parts = Story.Game.Services.GetService<CreationParts>()!;
        return parts.Find(inn, id);
    }

    public bool IsOpen(string id) => Story.Game.World.Get<Toggle>(Part(id)).Open;

    /// <summary>Whether the elevator's platform stands still at its stop on the level.</summary>
    public bool IsPlatformAt(int level)
    {
        Entity platform = Part(Platform);
        Elevator elevator = Story.Game.World.Get<Elevator>(platform);
        JointPose pose = Story.Game.World.Get<JointPose>(platform);
        int stop = Array.IndexOf(elevator.Levels, level);
        return stop == elevator.Stop && MathF.Abs(pose.Position - pose.Target) < 0.01f;
    }

    public bool WalkTo(Vector3 point, float seconds = 40) => Street.WalkTo(point, seconds);

    public Vector3 FoxPlace => Street.GetPosition(Fox);

    /// <summary>
    /// Clicks the part <paramref name="above"/> the point it is used from (a button's foot), seen from <paramref name="distance"/> back
    /// along <paramref name="forward"/>.
    /// </summary>
    public bool Click(string id, Vector3 forward, float distance, Func<bool> done, float seconds = 15, float above = 0)
    {
        Entity part = Part(id);
        Vector3 point = InteractionSystem.GetUsePoint(Story.Game.World, part);
        point.Y += above;
        return Story.ClickAt(point, forward, distance, done, seconds);
    }

    /// <summary>Clicks the door and lets it swing open; whether it opened.</summary>
    public bool OpenDoor(string id, Vector3 forward, float distance, float seconds = 15)
    {
        bool opened = Click(id, forward, distance, () => IsOpen(id), seconds);
        Story.Run(1.5f);
        return opened;
    }

    /// <summary>Takes the cellar key from the inn's room, as the story does.</summary>
    public bool TakeTheKey()
    {
        Entity fox = Fox;
        return Story.Click("Cellar Key", () => Story.Inventories.FindItemWithTag(fox, CellarKey) is not null);
    }

    public void Dispose() => Street.Dispose();
}
