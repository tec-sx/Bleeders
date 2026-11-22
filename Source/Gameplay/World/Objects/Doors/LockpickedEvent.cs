using Arch.Core;

using FlickerVoid.Engine.Attributes.Checks;

namespace Bleeders.Gameplay.World.Objects.Doors;

/// <summary>Someone tried to pick a lock; the check's result says whether it opened and how the roll went.</summary>
public readonly record struct LockpickedEvent(Entity Picker, Entity Lock, CheckResult Result);
