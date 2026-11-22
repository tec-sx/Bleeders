using Avalonia.Controls;
using Avalonia.Markup.Xaml;

namespace Bleeders.Interface.Hub.Inventory;

public sealed partial class InventoryPageView : UserControl
{
    public InventoryPageView() => AvaloniaXamlLoader.Load(this);
}
