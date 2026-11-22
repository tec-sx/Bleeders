using Avalonia.Controls;
using Avalonia.Markup.Xaml;

namespace Bleeders.Interface.Hub;

public sealed partial class HubView : UserControl
{
    public HubView() => AvaloniaXamlLoader.Load(this);
}
