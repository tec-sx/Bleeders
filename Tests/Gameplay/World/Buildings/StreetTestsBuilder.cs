using System.Numerics;

using Arch.Core;

using Bleeders.Gameplay.Tests.Story;

using FlickerVoid.Engine.Cameras.Cutaways;
using FlickerVoid.Engine.Cameras.Directing;
using FlickerVoid.Engine.Characters.Movement;
using FlickerVoid.Engine.Rendering.Extraction;

namespace Bleeders.Gameplay.Tests.World.Buildings;

/// <summary>Plays the main scene as <see cref="StoryLoopTestsBuilder"/> does and walks the fox about the street's buildings.</summary>
internal sealed class StreetTestsBuilder : IDisposable
{
    public StoryLoopTestsBuilder Story { get; } = new();

    public CutawaySystem Cutaways => Story.Game.Services.GetService<CutawaySystem>()!;

    public CameraDirector Director => Story.Game.Services.GetService<CameraDirector>()!;

    public StreetTestsBuilder Start()
    {
        Story.Start();
        return this;
    }

    /// <summary>Sends the fox walking to <paramref name="point"/> and plays until it stands within half a metre of it.</summary>
    public bool WalkTo(Vector3 point, float seconds = 40)
    {
        Entity fox = Story.Fox;
        Story.Game.Events.Send(new MoveCommand(fox, point));
        return Story.RunUntil(() => Vector3.Distance(GetPosition(fox), point) < 0.5f, seconds);
    }

    public Vector3 GetPosition(Entity entity) => Story.Game.World.Get<FlickerVoid.Engine.Runtime.Components.Transform>(entity).Position;

    public bool IsShadowOnly(string name) => Story.Game.World.Has<ShadowOnly>(Story.Find(name));

    public void Dispose() => Story.Dispose();
}
