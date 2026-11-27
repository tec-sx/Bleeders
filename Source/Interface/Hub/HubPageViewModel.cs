using Arch.Core;

using CommunityToolkit.Mvvm.ComponentModel;

using FlickerVoid.Engine.Gameplay.Localization;
using FlickerVoid.Engine.Runtime.Localization;
using Bleeders.Interface.Localization;

namespace Bleeders.Interface.Hub;

/// <summary>One page of the hub, filled from the protagonist's state when it is shown and kept up while it shows, in the chosen language.</summary>
public abstract partial class HubPageViewModel : ObservableObject
{
    private readonly LocalizedText _title;

    protected HubPageViewModel(LocalizedText title, TextLocalizer localizer)
    {
        _title = title;
        Localizer = localizer;
        Labels = new InterfaceLabels(localizer);
        localizer.Changed += HandleLanguageChanged;
    }

    public string Title => Localizer.Get(_title);

    public InterfaceLabels Labels { get; }

    [ObservableProperty]
    public partial bool IsSelected { get; private set; }

    /// <summary>Whose state the page shows while it is selected; <see cref="Entity.Null"/> otherwise.</summary>
    protected Entity Owner { get; private set; } = Entity.Null;

    protected TextLocalizer Localizer { get; }

    public void Show(Entity owner)
    {
        Owner = owner;
        IsSelected = true;
        Refresh();
    }

    public void Hide()
    {
        Owner = Entity.Null;
        IsSelected = false;
    }

    protected abstract void Refresh();

    /// <summary>Refreshes for a change to <paramref name="changed"/> when the page shows its state.</summary>
    protected void RefreshFor(Entity changed)
    {
        if (IsSelected && changed == Owner)
        {
            Refresh();
        }
    }

    private void HandleLanguageChanged()
    {
        OnPropertyChanged(nameof(Title));
        RefreshFor(Owner);
    }
}
