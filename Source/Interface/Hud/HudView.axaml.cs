using Avalonia.Controls;
using Avalonia.Markup.Xaml;

namespace Bleeders.Interface.Hud;

public sealed partial class HudView : UserControl
{
    public HudView() => AvaloniaXamlLoader.Load(this);
}
