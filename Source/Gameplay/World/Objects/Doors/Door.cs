using FlickerVoid.Engine.Runtime.Components;
using FlickerVoid.Engine.Runtime.Entities;

namespace Bleeders.Gameplay.World.Objects.Doors;

/// <summary>A swinging door while playing: how it swings, its hinge and where it stands.</summary>
[EntityClass]
public readonly partial struct Door
{
    public partial ref DoorSwing Swing { get; }

    public partial ref DoorHinge Hinge { get; }

    public partial ref Transform Transform { get; }
}
