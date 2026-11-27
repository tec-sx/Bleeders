using System.Numerics;

using Arch.Core;

using Bleeders.Gameplay.Tests.Story;

using FlickerVoid.Engine.Audio.Devices;
using FlickerVoid.Engine.Audio.Music;
using FlickerVoid.Engine.Audio.Sounds;
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

    public AudioSystem Audio => Story.Game.Services.GetService<AudioSystem>()!;

    public IMusicPlayer Music => Story.Game.Services.GetService<IMusicPlayer>()!;

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

    /// <summary>Steers the fox along <paramref name="direction"/> for <paramref name="seconds"/>, then stops it.</summary>
    public void Steer(Vector3 direction, float seconds)
    {
        Entity fox = Story.Fox;
        Story.Game.Events.Send(new SteerCommand(fox, direction));
        Story.Run(seconds);
        Story.Game.Events.Send(new SteerCommand(fox, Vector3.Zero));
        Story.Run(0.5f);
    }

    public Vector3 GetPosition(Entity entity) => Story.Game.World.Get<FlickerVoid.Engine.Runtime.Components.Transform>(entity).Position;

    public bool IsShadowOnly(string name) => Story.Game.World.Has<ShadowOnly>(Story.Find(name));

    /// <summary>The loudest sample the game plays over the next second, heard through a device without speakers.</summary>
    public float Hear(float seconds = 1)
    {
        NullAudioDevice device = new(Audio.Mixer);
        float[] heard = device.Render(TimeSpan.FromSeconds(seconds));
        return heard.Max(MathF.Abs);
    }

    public void Dispose() => Story.Dispose();
}
