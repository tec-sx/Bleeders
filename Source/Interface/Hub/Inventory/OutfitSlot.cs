using FlickerVoid.Engine.Gameplay.Localization;
using FlickerVoid.Engine.Gameplay.Tags;

namespace Bleeders.Interface.Hub.Inventory;

/// <summary>An outfit slot as the inventory page lists it.</summary>
public sealed record OutfitSlot(Tag Slot, LocalizedText Label);
