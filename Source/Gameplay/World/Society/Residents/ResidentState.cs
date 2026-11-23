using System.Numerics;
using Arch.Core;

namespace Bleeders.Gameplay.World.Society.Residents;

public struct ResidentState
{
    public Vector3 Home { get; set; }

    public float RestLeft { get; set; }

    public Entity Watching { get; set; }
}
