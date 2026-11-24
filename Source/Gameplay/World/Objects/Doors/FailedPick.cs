namespace Bleeders.Gameplay.World.Objects.Doors;

/// <summary>The picker's bonus (the lockpick check without its roll) when a pick of this lock last failed; it is tried again only with a higher one.</summary>
public struct FailedPick
{
    public int Bonus { get; set; }
}
