using CommunityToolkit.Mvvm.Input;

namespace Bleeders.Interface.Hub.Inventory;

/// <summary>An outfit slot: what is worn there, or what covers it, and whether the selected stack could be worn there.</summary>
public sealed partial class OutfitSlotViewModel(OutfitSlot slot, string label, string itemName, bool isBlocked, bool isSelected, bool canTake, Action<OutfitSlotViewModel> select)
{
    public OutfitSlot Slot { get; } = slot;

    /// <summary>The slot's name in the chosen language.</summary>
    public string Label { get; } = label;

    public string ItemName { get; } = itemName;

    public bool IsBlocked { get; } = isBlocked;

    public bool IsSelected { get; } = isSelected;

    public bool CanTake { get; } = canTake;

    [RelayCommand]
    private void Select() => select(this);
}
