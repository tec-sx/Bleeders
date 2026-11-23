using FlickerVoid.Engine.Runtime.Components;
using FlickerVoid.Engine.Runtime.Entities;

namespace Bleeders.Gameplay.World.Society.Residents;

/// <summary>A resident while playing: its settings, what it is doing and where it stands.</summary>
[EntityClass]
public readonly partial struct Dweller
{
    public partial ref Resident Resident { get; }

    public partial ref ResidentState State { get; }

    public partial ref Transform Transform { get; }
}
