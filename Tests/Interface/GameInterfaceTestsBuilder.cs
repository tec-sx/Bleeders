using System.Text.Json;

using Arch.Core;

using FlickerVoid.Engine.Dialogue.Conversations;
using FlickerVoid.Engine.Flow;
using FlickerVoid.Engine.Flow.Graphs;
using FlickerVoid.Engine.Platform.Input;
using FlickerVoid.Engine.Platform.Windowing;
using FlickerVoid.Engine.Runtime.Content;
using FlickerVoid.Engine.Runtime.Data;
using FlickerVoid.Engine.Runtime.Loop;
using FlickerVoid.Engine.Runtime.Scenes;
using FlickerVoid.Engine.Runtime.Serialization;
using FlickerVoid.Engine.Ui.Surfaces;
using Bleeders.Gameplay;

using Microsoft.Extensions.Logging.Abstractions;

using Silk.NET.Input;

namespace Bleeders.Interface.Tests;

/// <summary>
/// Builds the game's interface over a playing game with Bleeders' gameplay, its content and its controls, read from a keyboard the
/// test presses each frame, with a window that only records being closed: a protagonist and an innkeeper to talk to.
/// </summary>
internal sealed class GameInterfaceTestsBuilder : IDisposable
{
    private readonly IGameWindow _window = Substitute.For<IGameWindow>();
    private readonly InputState _input = new();
    private DataAssets? _data;
    private Entity _protagonist;
    private Entity _innkeeper;

    public Game Game { get; } = new(NullLoggerFactory.Instance);

    public InputActions Actions { get; private set; } = null!;

    public bool WindowClosed { get; private set; }

    public GameInterface Build()
    {
        UiPlatform.Start<InterfaceApplication>();
        _window.When(window => window.Close()).Do(_ => WindowClosed = true);
        Game.Services.AddService(_window);
        string folder = FindContent();
        ContentManager content = new(folder);
        _data = new DataAssets(content, Game.Data);
        Game.Services.AddService(_data);
        Game.AddInput();
        Actions = Game.AddInputActions(_input);
        Game.AddFlow();
        FlowGraphs graphs = new(content, Game.Data);
        Game.Services.AddService(graphs);
        GameplayModule gameplay = new();
        gameplay.Add(Game);
        InterfaceModule ui = new();
        ui.Add(Game);
        Actions.Map = _data.Load<InputMap>("Input/Controls.fvdata");
        string json = """
            { "entities": [
                { "name": "Fox", "components": { "transform": {}, "protagonist": {}, "interactor": { "reach": 2 } } },
                { "name": "Innkeeper", "components": { "transform": { "position": [1, 0, 0] },
                    "dialogueParticipant": { "speaker": "Character.Innkeeper", "dialogue": "Dialogues/Innkeeper.fvflow" } } } ] }
            """;
        SceneData scene = JsonSerializer.Deserialize(json, RuntimeJsonContext.Default.SceneData)!;
        IReadOnlyList<Entity> spawned = Game.Spawner.Spawn(scene, Game.World);
        _protagonist = spawned[0];
        _innkeeper = spawned[1];
        GameInterface gameInterface = new(Game);
        Game.FrameSystems.Add(gameInterface);
        Game.Lifecycle.Play(Game.World);
        Frame();
        return gameInterface;
    }

    /// <summary>The protagonist starts talking to the innkeeper, as its Talk offer does, and a frame goes by.</summary>
    public void Talk()
    {
        DialogueSystem dialogue = Game.Services.GetService<DialogueSystem>()!;
        dialogue.Start(_protagonist, _innkeeper);
        Frame();
    }

    /// <summary>Presses the key for a frame and lets it go the next.</summary>
    public void Press(Key key)
    {
        _input.PressKey(key);
        Frame();
        _input.ReleaseKey(key);
        Frame();
    }

    /// <summary>Holds the key down for a frame and lets it go after it.</summary>
    public void Hold(Key key)
    {
        _input.PressKey(key);
        Frame();
        _input.ReleaseKey(key);
    }

    public void Dispose()
    {
        Game.Dispose();
        _data?.Dispose();
    }

    private void Frame()
    {
        Game.Tick(Game.Loop.FixedStep);
        _input.EndFrame();
    }

    private static string FindContent()
    {
        DirectoryInfo? folder = new(AppContext.BaseDirectory);
        while (folder is not null)
        {
            string content = Path.Combine(folder.FullName, "Content");
            string project = Path.Combine(folder.FullName, "Bleeders.fvproject");
            if (File.Exists(project) && Directory.Exists(content))
            {
                return content;
            }

            folder = folder.Parent;
        }

        throw new DirectoryNotFoundException("No Bleeders.fvproject above " + AppContext.BaseDirectory);
    }
}
