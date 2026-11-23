using System.Numerics;

using FlickerVoid.Engine.Cameras.Cutaways;
using FlickerVoid.Engine.Physics;
using FlickerVoid.Engine.Platform.Input;
using FlickerVoid.Engine.Platform.Windowing;
using FlickerVoid.Engine.Rendering.Cameras;
using FlickerVoid.Engine.Rendering.Extraction;
using FlickerVoid.Engine.Runtime.Geometry;

using Silk.NET.Maths;

namespace Bleeders.Gameplay.Protagonist.Control;

using World = Arch.Core.World;

/// <summary>Finds what the mouse points at: a ray from the drawn camera (the director's posed view, else the first camera) through the mouse, met by the physics world, passing through what the camera can't see. A game service.</summary>
public sealed class PointerPicker(World world, IInputState input, IGameWindow window, IPhysicsSystem physics, CutawaySystem cutaways)
{
    private const float MaxDistance = 500;

    /// <summary>How many unseen bodies a pick may pass through, such as a cut-away wall, ceiling and roof.</summary>
    private const int MaxUnseen = 16;

    /// <summary>How far past an unseen body's hit the ray starts again.</summary>
    private const float Step = 0.001f;

    public bool TryPick(out RayHit hit)
    {
        hit = default;
        RenderCamera? camera = ActiveCamera.Find(world);
        if (camera is not { } drawn || window.Size.X <= 0 || window.Size.Y <= 0)
        {
            return false;
        }

        CameraView view = CameraView.Create(drawn.Camera, drawn.Transform, window.Size.X, window.Size.Y);
        Ray ray = Ray.FromViewProjection(view.ViewProjection, input.MousePosition, window.Size.X, window.Size.Y);

        return PickSeen(ray, out hit);
    }

    /// <summary>The first hit the camera can see: bodies that only cast shadows or stand in front of the cutaway are passed through.</summary>
    private bool PickSeen(Ray ray, out RayHit hit)
    {
        if (!physics.RayCast(ray, MaxDistance, out hit))
        {
            return false;
        }

        float travelled = 0;
        for (int passed = 0; passed < MaxUnseen && cutaways.IsUnseen(hit.Entity, hit.Point, ray.Direction); passed++)
        {
            travelled += hit.Distance + Step;
            Vector3 origin = hit.Point + (ray.Direction * Step);
            Ray beyond = new(origin, ray.Direction);
            if (!physics.RayCast(beyond, MaxDistance - travelled, hit.Entity, out hit))
            {
                return false;
            }
        }

        hit = hit with { Distance = hit.Distance + travelled };
        return true;
    }
}
