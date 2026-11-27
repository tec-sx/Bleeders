using FlickerVoid.Engine.Runtime.Components;

namespace Bleeders.Gameplay.World.Objects.Doors;

public struct DoorSwing
{
    public DoorSwing()
    {
    }

    public float OpenAngle { get; set; } = 90;

    public float Speed { get; set; } = 120;

    /// <summary>The <c>sound</c> data asset played on the door when a lock blocks it; none when empty.</summary>
    [ContentFile(".fvdata")]
    public string RattleSound { get; set; } = string.Empty;

    /// <summary>The <c>sound</c> data asset played on the door as it starts to swing open or shut; none when empty.</summary>
    [ContentFile(".fvdata")]
    public string SwingSound { get; set; } = string.Empty;
}
