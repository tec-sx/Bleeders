using System.Collections.ObjectModel;

using FlickerVoid.Engine.Gameplay.Tags;
using FlickerVoid.Engine.Runtime.Events;
using FlickerVoid.Engine.Runtime.Localization;
using FlickerVoid.Engine.Story.Quests;

namespace Bleeders.Interface.Hud.Quests;

/// <summary>The HUD's quest tracker: up to three active quests, the most recently changed first, each with its current objective.</summary>
public sealed class QuestTrackerViewModel
{
    public const int MaxQuests = 3;

    private readonly QuestSystem _quests;
    private readonly List<Tag> _order = [];
    private readonly TextLocalizer _localizer;

    public QuestTrackerViewModel(QuestSystem quests, EventBus events, TextLocalizer localizer)
    {
        _quests = quests;
        _localizer = localizer;
        events.Handle((in QuestChangedEvent changed) => HandleChanged(changed.Quest));
        localizer.Changed += Rebuild;
        Rebuild();
    }

    public ObservableCollection<TrackedQuestViewModel> Quests { get; } = [];

    /// <summary>The first visible objective not done yet; empty when there is none.</summary>
    public string GetCurrentObjective(QuestDefinition quest)
    {
        foreach (QuestObjective objective in quest.Objectives)
        {
            if (_quests.IsObjectiveVisible(objective) && !_quests.IsObjectiveComplete(objective.Fact))
            {
                return _localizer.Get(objective.Description);
            }
        }

        return string.Empty;
    }

    private void HandleChanged(Tag quest)
    {
        _order.Remove(quest);
        _order.Insert(0, quest);
        Rebuild();
    }

    private void Rebuild()
    {
        IReadOnlyList<QuestDefinition> active = _quests.GetQuests(QuestState.Active);
        List<QuestDefinition> shown = [];
        foreach (Tag id in _order)
        {
            QuestDefinition? quest = active.FirstOrDefault(candidate => candidate.Id == id);
            if (quest is not null)
            {
                shown.Add(quest);
            }
        }

        foreach (QuestDefinition quest in active)
        {
            if (!shown.Contains(quest))
            {
                shown.Add(quest);
            }
        }

        Quests.Clear();
        int count = Math.Min(shown.Count, MaxQuests);
        for (int index = 0; index < count; index++)
        {
            QuestDefinition quest = shown[index];
            string objective = GetCurrentObjective(quest);
            string name = DisplayNames.Of(quest, _localizer);
            Quests.Add(new TrackedQuestViewModel(name, objective));
        }
    }
}
