using Avalonia.Controls;
using Avalonia.Markup.Xaml;

namespace Bleeders.Interface.Hud.Minimap;

public sealed partial class MinimapView : UserControl
{
    public MinimapView() => AvaloniaXamlLoader.Load(this);
}
