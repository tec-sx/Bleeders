using System.Numerics;

using Arch.System;

using FlickerVoid.Engine.Cameras.Directing;
using FlickerVoid.Engine.Gameplay.Tags;
using FlickerVoid.Engine.Platform.Input;
using FlickerVoid.Engine.Platform.Windowing;
using FlickerVoid.Engine.Runtime.Loop;

using Silk.NET.Maths;

namespace Bleeders.Gameplay.Protagonist.Control;

using World = Arch.Core.World;

/// <summary>
/// Hands the camera director what the player does with the camera, which is little, as it is directed: the pointer, which look
/// layers drift towards, and <see cref="Skip"/> (Space), which skips a skippable cutscene shot.
/// </summary>
public sealed class CameraControlSystem : BaseSystem<World, GameTime>
{
    public static readonly Tag Skip = new("Input.Skip");

    private readonly InputActions _actions;
    private readonly IInputState _input;
    private readonly IGameWindow _window;
    private readonly CameraDirector _director;

    public CameraControlSystem(World world, InputActions actions, IInputState input, IGameWindow window, CameraDirector director)
        : base(world)
    {
        _actions = actions;
        _input = input;
        _window = window;
        _director = director;
    }

    public override void Update(in GameTime t)
    {
        _director.Pointer = null;
        if (_window.Size.X > 0 && _window.Size.Y > 0)
        {
            Vector2 mouse = _input.MousePosition;
            float x = (2 * mouse.X / _window.Size.X) - 1;
            float y = 1 - (2 * mouse.Y / _window.Size.Y);
            _director.Pointer = new Vector2(x, y);
        }

        if (_actions.WasPressed(Skip))
        {
            _director.Skip();
        }
    }
}
