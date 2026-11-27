using System.Collections.ObjectModel;

using FlickerVoid.Engine.Dialogue.Conversations;
using FlickerVoid.Engine.Interaction.Interactors;
using FlickerVoid.Engine.Maps.Navigation;
using FlickerVoid.Engine.Runtime.Localization;
using FlickerVoid.Engine.Runtime.Loop;
using FlickerVoid.Engine.Story.Quests;
using Bleeders.Gameplay.Protagonist.Resources.Knowledge.Scanning;
using Bleeders.Interface.Hud.Conversations;
using Bleeders.Interface.Hud.Minimap;
using Bleeders.Interface.Hud.Prompts;
using Bleeders.Interface.Hud.Quests;
using Bleeders.Interface.Hud.Scanning;
using Bleeders.Interface.Localization;

namespace Bleeders.Interface.Hud;

/// <summary>
/// The HUD in its three places: information at the top left (the tracked quests), the action at the bottom centre (the
/// conversation's dialogue box, the scan card in scan mode, or else the prompt for what the pointer is over) and navigation at the bottom right (the minimap).
/// </summary>
public sealed class HudViewModel
{
    public HudViewModel(Game game)
    {
        QuestSystem quests = game.Services.GetService<QuestSystem>()!;
        DialogueSystem dialogue = game.Services.GetService<DialogueSystem>()!;
        InteractionSystem interaction = game.Services.GetService<InteractionSystem>()!;
        TextLocalizer localizer = game.Localization;
        Labels = new InterfaceLabels(localizer);
        Tracker = new QuestTrackerViewModel(quests, game.Events, localizer);
        Dialogue = new DialogueBoxViewModel(dialogue, game.World, game.Events, localizer);
        Prompt = new InteractionPromptViewModel(interaction, dialogue, game.World, localizer);
        MapSystem? maps = game.Services.GetService<MapSystem>();
        Minimap = new MinimapViewModel(game.World, maps);
        ScannerSystem? scanner = game.Services.GetService<ScannerSystem>();
        if (scanner is not null)
        {
            Scan = new ScanCardViewModel(scanner, game.World, localizer);
        }
    }

    /// <summary>The fixed labels, in the chosen language.</summary>
    public InterfaceLabels Labels { get; }

    public QuestTrackerViewModel Tracker { get; }

    public ObservableCollection<TrackedQuestViewModel> Quests => Tracker.Quests;

    public DialogueBoxViewModel Dialogue { get; }

    public InteractionPromptViewModel Prompt { get; }

    public MinimapViewModel Minimap { get; }

    /// <summary>The scan card, where there is a scanner (a window to point in).</summary>
    public ScanCardViewModel? Scan { get; }
}
