using System.Collections.ObjectModel;

using FlickerVoid.Engine.Gameplay.Localization;
using FlickerVoid.Engine.Story.Knowledge;
using FlickerVoid.Engine.Story.Quests;
using FlickerVoid.Engine.Runtime.Loop;
using Bleeders.Interface.Localization;

namespace Bleeders.Interface.Hub.Journal;

/// <summary>The hub's journal in three sections: active quests with their visible objectives, finished quests, and knowledge.</summary>
public sealed class JournalPageViewModel : HubPageViewModel
{
    private readonly QuestSystem _quests;
    private readonly KnowledgeSystem _knowledge;

    public JournalPageViewModel(Game game)
        : base(InterfaceTexts.Journal, game.Localization)
    {
        _quests = game.Services.GetService<QuestSystem>()!;
        _knowledge = game.Services.GetService<KnowledgeSystem>()!;
        game.Events.Handle((in QuestChangedEvent _) => RefreshFor(Owner));
    }

    public ObservableCollection<JournalQuestViewModel> ActiveQuests { get; } = [];

    /// <summary>Completed and failed quests.</summary>
    public ObservableCollection<JournalEntryViewModel> FinishedQuests { get; } = [];

    public ObservableCollection<JournalEntryViewModel> Knowledge { get; } = [];

    protected override void Refresh()
    {
        ActiveQuests.Clear();
        foreach (QuestDefinition quest in _quests.GetQuests(QuestState.Active))
        {
            List<string> objectives = [];
            foreach (QuestObjective objective in quest.Objectives)
            {
                if (_quests.IsObjectiveVisible(objective))
                {
                    objectives.Add(Localizer.Get(objective.Description));
                }
            }

            string name = DisplayNames.Of(quest, Localizer);
            string description = Localizer.Get(quest.Display.ShortDescription);
            ActiveQuests.Add(new JournalQuestViewModel(name, description, objectives));
        }

        FinishedQuests.Clear();
        AddFinished(QuestState.Completed, InterfaceTexts.Completed);
        AddFinished(QuestState.Failed, InterfaceTexts.Failed);

        Knowledge.Clear();
        foreach (KnowledgeDefinition known in _knowledge.GetKnown())
        {
            string name = DisplayNames.Of(known, Localizer);
            string description = Localizer.Get(known.Display.ShortDescription);
            Knowledge.Add(new JournalEntryViewModel(name, description));
        }
    }

    private void AddFinished(QuestState state, LocalizedText label)
    {
        string shown = Localizer.Get(label);
        foreach (QuestDefinition quest in _quests.GetQuests(state))
        {
            string name = DisplayNames.Of(quest, Localizer);
            FinishedQuests.Add(new JournalEntryViewModel(name, shown));
        }
    }
}
