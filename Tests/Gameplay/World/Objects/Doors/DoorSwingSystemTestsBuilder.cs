using System.Text.Json;

using Arch.Core;

using FlickerVoid.Engine.Audio.Sounds;
using FlickerVoid.Engine.Interaction.Toggles;
using FlickerVoid.Engine.Runtime.Loop;
using FlickerVoid.Engine.Runtime.Scenes;
using FlickerVoid.Engine.Runtime.Serialization;
using Bleeders.Gameplay.Serialization;
using Bleeders.Gameplay.World.Objects.Doors;

using Microsoft.Extensions.Logging.Abstractions;

namespace Bleeders.Gameplay.Tests.World.Objects.Doors;

/// <summary>A playing game with the door swing system and one door, whose toggle events the tests send themselves.</summary>
internal sealed class DoorSwingSystemTestsBuilder : IDisposable
{
    public DoorSwingSystemTestsBuilder()
    {
        Game.Spawner.Register("doorSwing", BleedersJsonContext.Default.DoorSwing);
        DoorSwingSystem doors = new(Game);
        Game.FixedSystems.Add(doors);
        Game.Lifecycle.Play(Game.World);
        Game.Events.Handle((in PlaySoundCommand command) => Sounds.Add(command));
    }

    public Game Game { get; } = new(NullLoggerFactory.Instance);

    /// <summary>The sounds asked for so far.</summary>
    public List<PlaySoundCommand> Sounds { get; } = [];

    /// <summary>A door with the swing settings' JSON members.</summary>
    public Entity SpawnDoor(string swing)
    {
        string json = $$"""{ "entities": [ { "name": "Door", "components": { "transform": {}, "doorSwing": { {{swing}} } } } ] }""";
        SceneData scene = JsonSerializer.Deserialize(json, RuntimeJsonContext.Default.SceneData)!;
        return Game.Spawner.Spawn(scene, Game.World)[0];
    }

    public void Toggle(Entity door, bool open)
    {
        ToggledEvent toggled = new(door, open, Entity.Null);
        Game.Events.Send(in toggled);
        Game.Tick(GameLoop.DefaultFixedStep);
    }

    public void Block(Entity door)
    {
        ToggleBlockedEvent blocked = new(door, Entity.Null, true);
        Game.Events.Send(in blocked);
        Game.Tick(GameLoop.DefaultFixedStep);
    }

    public void Dispose() => Game.Dispose();
}
