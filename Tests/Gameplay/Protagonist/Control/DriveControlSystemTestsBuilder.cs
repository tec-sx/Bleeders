using System.Numerics;
using System.Text;
using System.Text.Json;

using Arch.Core;

using Bleeders.Gameplay.Protagonist.Control;

using FlickerVoid.Engine.Flow;
using FlickerVoid.Engine.Flow.Graphs;
using FlickerVoid.Engine.Platform.Input;
using FlickerVoid.Engine.Platform.Windowing;
using FlickerVoid.Engine.Rendering;
using FlickerVoid.Engine.Runtime.Content;
using FlickerVoid.Engine.Runtime.Data;
using FlickerVoid.Engine.Runtime.Loop;
using FlickerVoid.Engine.Runtime.Scenes;
using FlickerVoid.Engine.Runtime.Serialization;
using FlickerVoid.Engine.Vehicles;
using FlickerVoid.Engine.Vehicles.Serialization;

using Microsoft.Extensions.Logging.Abstractions;

using Silk.NET.Input;
using Silk.NET.Maths;

namespace Bleeders.Gameplay.Tests.Protagonist.Control;

/// <summary>
/// Builds a playing game with Bleeders' gameplay over a content folder holding a new car's file, the game's driving keys, a wide
/// floor, the car on it at the origin and the fox standing on its left.
/// </summary>
internal sealed class DriveControlSystemTestsBuilder : IDisposable
{
    private const string Scene = """
        { "entities": [
          { "name": "Camera", "components": { "transform": { "position": [0, 6, 9], "rotation": [-0.2588, 0, 0, 0.9659] },
            "camera": { "projection": "Orthographic", "orthographicHeight": 6.5 } } },
          { "name": "Floor", "components": { "transform": { "position": [0, -0.5, 0] }, "greybox": { "size": [400, 0.5, 400] } } },
          { "name": "Car", "components": { "transform": {}, "vehicle": { "file": "Car.fvvehicle" } } },
          { "name": "Fox", "components": { "transform": { "position": [2, 0, 0] }, "protagonist": {}, "characterMover": { "speed": 2 }, "interactor": {} } }
        ] }
        """;

    private readonly string _folder;
    private readonly HashSet<Key> _down = [];
    private readonly DataAssets _data;

    public DriveControlSystemTestsBuilder()
    {
        string folderName = Guid.NewGuid().ToString("N");
        string tempPath = Path.GetTempPath();
        _folder = Path.Combine(tempPath, folderName);
        Directory.CreateDirectory(_folder);
        ContentManager content = new(_folder);
        VehicleFile car = VehicleFile.Create(VehicleType.Car);
        string json = JsonSerializer.Serialize(car, VehiclesJsonContext.Default.VehicleFile);
        content.Write("Car.fvvehicle", Encoding.UTF8.GetBytes(json));

        IGameWindow window = Substitute.For<IGameWindow>();
        window.Size.Returns(new Vector2D<int>(100, 100));
        IInputState input = Substitute.For<IInputState>();
        input.IsDown(Arg.Any<Key>()).Returns(call => _down.Contains(call.Arg<Key>()));
        Game.Services.AddService(window);
        Game.Services.AddService(input);
        Game.Services.AddService<IContentManager>(content);
        InputActions actions = Game.AddInputActions(input);
        actions.Map = CreateMap();
        _data = new DataAssets(content, Game.Data);
        Game.Services.AddService(_data);
        Game.AddRendering();
        Game.AddFlow();
        FlowGraphs graphs = new(content, Game.Data);
        Game.Services.AddService(graphs);
        GameplayModule gameplay = new();
        gameplay.Add(Game);
        Game.Lifecycle.Play(Game.World);
        SceneData scene = JsonSerializer.Deserialize(Scene, RuntimeJsonContext.Default.SceneData)!;
        IReadOnlyList<Entity> entities = Game.Spawner.Spawn(scene, Game.World);
        Car = entities[2];
        Fox = entities[3];
        Run(0.1f);
    }

    public Game Game { get; } = new(NullLoggerFactory.Instance);

    public Entity Car { get; }

    public Entity Fox { get; }

    public void Hold(Key key) => _down.Add(key);

    public void Release(Key key) => _down.Remove(key);

    /// <summary>Presses and lets go of a key, a few steps apart.</summary>
    public void Tap(Key key)
    {
        Hold(key);
        Run(0.05f);
        Release(key);
        Run(0.05f);
    }

    public void Run(float seconds)
    {
        int steps = (int)MathF.Ceiling(seconds * 60);
        for (int step = 0; step < steps; step++)
        {
            Game.Tick(Game.Loop.FixedStep);
        }
    }

    public void Dispose()
    {
        Game.Dispose();
        _data.Dispose();
        Directory.Delete(_folder, recursive: true);
    }

    private static InputMap CreateMap()
    {
        InputActionDefinition move = new() { Id = DriveControlSystem.Move };
        move.Bindings.Add(new InputBinding { Key = Key.W, Value = new Vector2(0, 1) });
        move.Bindings.Add(new InputBinding { Key = Key.S, Value = new Vector2(0, -1) });
        move.Bindings.Add(new InputBinding { Key = Key.A, Value = new Vector2(-1, 0) });
        move.Bindings.Add(new InputBinding { Key = Key.D, Value = new Vector2(1, 0) });
        InputActionDefinition drive = new() { Id = DriveControlSystem.Drive };
        drive.Bindings.Add(new InputBinding { Key = Key.E });
        InputActionDefinition handBrake = new() { Id = DriveControlSystem.HandBrake };
        handBrake.Bindings.Add(new InputBinding { Key = Key.Space });
        InputMap map = new();
        map.Actions.Add(move);
        map.Actions.Add(drive);
        map.Actions.Add(handBrake);
        return map;
    }
}
