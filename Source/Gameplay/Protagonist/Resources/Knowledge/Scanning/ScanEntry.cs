using FlickerVoid.Engine.Gameplay.Conditions;
using FlickerVoid.Engine.Gameplay.Localization;

namespace Bleeders.Gameplay.Protagonist.Resources.Knowledge.Scanning;

/// <summary>One line a scan reveals, shown only while its conditions pass (a Perception check, a clue known).</summary>
public sealed class ScanEntry
{
    public LocalizedText Label { get; set; } = new();

    public LocalizedText Text { get; set; } = new();

    public ConditionSet VisibleWhen { get; set; } = new();
}
