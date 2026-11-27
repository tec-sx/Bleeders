using Arch.Core;

using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;

using FlickerVoid.Engine.Audio.Mixing;
using FlickerVoid.Engine.Audio.Music;
using FlickerVoid.Engine.Audio.Sounds;
using FlickerVoid.Engine.Gameplay.Tags;
using FlickerVoid.Engine.Platform.Input;
using FlickerVoid.Engine.Runtime.Loop;
using Bleeders.Interface.Hub.Character;
using Bleeders.Interface.Hub.Inventory;
using Bleeders.Interface.Hub.Journal;

namespace Bleeders.Interface.Hub;

/// <summary>
/// The hub menu over the game: three pages (Inventory, Character, Journal) of the protagonist, one shown at a time, switched in a
/// ring and reopened on the page last shown. While open it holds the game paused and its own input context, so only its actions are
/// read, and its song plays over the level's music, which continues where it stopped when the hub closes.
/// </summary>
public sealed partial class HubViewModel : ObservableObject
{
    /// <summary>The reason the hub holds the game paused.</summary>
    public static readonly Tag PauseReason = new("Pause.Hub");

    /// <summary>The input context the hub holds while open: Hub, Next and Previous Page, and Back.</summary>
    public static readonly Tag InputContext = new("Input.Context.Hub");

    /// <summary>The cue pushed over the level's music while the hub is open.</summary>
    public const string Music = "Music/Hub.fvmusic";

    private static readonly QueryDescription Protagonists = new QueryDescription().WithAll<Gameplay.Protagonist.Protagonist>();

    private readonly Game _game;
    private readonly World _world;
    private readonly InputActions? _actions;
    private readonly IMusicPlayer? _music;
    private readonly AudioSystem? _audio;
    private readonly List<HubPageViewModel> _pages;
    private int _selectedIndex;

    public HubViewModel(Game game)
    {
        _game = game;
        _world = game.World;
        _actions = game.Services.GetService<InputActions>();
        _music = game.Services.GetService<IMusicPlayer>();
        _audio = game.Services.GetService<AudioSystem>();
        Inventory = new InventoryPageViewModel(game);
        Character = new CharacterPageViewModel(game);
        Journal = new JournalPageViewModel(game);
        _pages = [Inventory, Character, Journal];
        SelectedPage = Inventory;
    }

    public IReadOnlyList<HubPageViewModel> Pages => _pages;

    public InventoryPageViewModel Inventory { get; }

    public CharacterPageViewModel Character { get; }

    public JournalPageViewModel Journal { get; }

    [ObservableProperty]
    [NotifyPropertyChangedFor(nameof(ShownPage))]
    public partial bool IsOpen { get; private set; }

    [ObservableProperty]
    [NotifyPropertyChangedFor(nameof(ShownPage))]
    public partial HubPageViewModel SelectedPage { get; private set; }

    /// <summary>The selected page while the hub is open, else none, so a closed hub holds no page view (nor a stage being drawn).</summary>
    public HubPageViewModel? ShownPage => IsOpen ? SelectedPage : null;

    public void Toggle()
    {
        if (IsOpen)
        {
            Close();
        }
        else
        {
            Open();
        }
    }

    public void Open()
    {
        if (!IsOpen)
        {
            _music?.Push(Music);
            _audio?.SetMixState(MixSet.HubOpen, true);
        }

        IsOpen = true;
        _game.Pause(PauseReason);
        _actions?.Enter(InputContext);
        Select(_selectedIndex);
    }

    [RelayCommand]
    public void Close()
    {
        if (IsOpen)
        {
            _music?.Pop();
            _audio?.SetMixState(MixSet.HubOpen, false);
        }

        IsOpen = false;
        _game.Resume(PauseReason);
        _actions?.Leave(InputContext);
        SelectedPage.Hide();
    }

    [RelayCommand]
    private void NextPage() => Select((_selectedIndex + 1) % Pages.Count);

    [RelayCommand]
    private void PreviousPage() => Select((_selectedIndex + Pages.Count - 1) % Pages.Count);

    [RelayCommand]
    private void SelectPage(HubPageViewModel page)
    {
        int index = _pages.IndexOf(page);
        Select(index);
    }

    private void Select(int index)
    {
        SelectedPage.Hide();
        _selectedIndex = index;
        SelectedPage = Pages[index];
        if (IsOpen)
        {
            Entity protagonist = FindProtagonist();
            SelectedPage.Show(protagonist);
        }
    }

    private Entity FindProtagonist()
    {
        Entity found = Entity.Null;
        _world.Query(in Protagonists, (Entity entity) =>
        {
            if (found == Entity.Null)
            {
                found = entity;
            }
        });
        return found;
    }
}
