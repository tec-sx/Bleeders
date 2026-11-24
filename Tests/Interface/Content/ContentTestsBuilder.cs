using FlickerVoid.Engine.Flow;
using FlickerVoid.Engine.Flow.Graphs;
using FlickerVoid.Engine.Platform.Input;
using FlickerVoid.Engine.Rendering;
using FlickerVoid.Engine.Runtime.Content;
using FlickerVoid.Engine.Runtime.Data;
using FlickerVoid.Engine.Runtime.Loop;
using FlickerVoid.Engine.Runtime.Saves;
using Bleeders.Gameplay;

using Microsoft.Extensions.Logging.Abstractions;

namespace Bleeders.Interface.Tests.Content;

/// <summary>Builds a game registering what the launcher's does, without a window, and checks the project's source content with it.</summary>
internal sealed class ContentTestsBuilder : IDisposable
{
    private readonly string _saves = Path.Combine(Path.GetTempPath(), Guid.NewGuid().ToString("N"));
    private readonly Game _game = new(NullLoggerFactory.Instance);
    private readonly DataAssets _data;

    public ContentTestsBuilder()
    {
        Folder = FindContent();
        ContentManager content = new(Folder);
        _data = new DataAssets(content, _game.Data);
        _game.Services.AddService(_data);
        _game.AddRendering();
        _game.AddFlow();
        _game.Services.AddService(new FlowGraphs(content, _game.Data));
        _game.AddInput();
        _game.AddSaves(new SaveSettings { Folder = _saves });
        GameplayModule gameplay = new();
        gameplay.Add(_game);
        InterfaceModule ui = new();
        ui.Add(_game);
    }

    public string Folder { get; }

    public IReadOnlyList<ContentProblem> Check() => ContentCheck.Run(_game, Folder);

    public void Dispose()
    {
        _game.Dispose();
        _data.Dispose();
        if (Directory.Exists(_saves))
        {
            Directory.Delete(_saves, recursive: true);
        }
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
