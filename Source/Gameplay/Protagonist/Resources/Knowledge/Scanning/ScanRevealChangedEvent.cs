using Arch.Core;

namespace Bleeders.Gameplay.Protagonist.Resources.Knowledge.Scanning;

/// <summary>Sent when scan mode starts or stops showing a scannable.</summary>
public readonly record struct ScanRevealChangedEvent(Entity Target, bool Revealed);
