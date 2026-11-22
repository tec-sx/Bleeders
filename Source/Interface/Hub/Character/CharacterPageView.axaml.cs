using Avalonia.Controls;
using Avalonia.Markup.Xaml;

namespace Bleeders.Interface.Hub.Character;

public sealed partial class CharacterPageView : UserControl
{
    public CharacterPageView() => AvaloniaXamlLoader.Load(this);
}
