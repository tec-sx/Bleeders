using System.Collections.ObjectModel;

using Arch.Core;

using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;

using FlickerVoid.Engine.Dialogue.Conversations;
using FlickerVoid.Engine.Runtime.Events;
using FlickerVoid.Engine.Runtime.Localization;

namespace Bleeders.Interface.Hud.Conversations;

/// <summary>The HUD's dialogue box: who speaks, the line, Next while the line waits, up to three choices, and Leave.</summary>
public sealed partial class DialogueBoxViewModel : ObservableObject
{
    private readonly DialogueSystem _dialogue;
    private readonly World _world;
    private readonly TextLocalizer _localizer;

    public DialogueBoxViewModel(DialogueSystem dialogue, World world, EventBus events, TextLocalizer localizer)
    {
        _dialogue = dialogue;
        _world = world;
        _localizer = localizer;
        localizer.Changed += Refresh;
        events.Handle((in ConversationStartedEvent _) => Refresh());
        events.Handle((in LineStartedEvent _) => Refresh());
        events.Handle((in LineFinishedEvent _) => Refresh());
        events.Handle((in ChoicesPresentedEvent _) => Refresh());
        events.Handle((in ConversationEndedEvent _) => Refresh());
    }

    [ObservableProperty]
    public partial bool IsOpen { get; private set; }

    [ObservableProperty]
    public partial string Speaker { get; private set; } = string.Empty;

    [ObservableProperty]
    public partial string Line { get; private set; } = string.Empty;

    [ObservableProperty]
    public partial bool CanAdvance { get; private set; }

    public ObservableCollection<DialogueChoiceViewModel> Choices { get; } = [];

    [RelayCommand]
    private void Advance()
    {
        _dialogue.Advance();
        Refresh();
    }

    [RelayCommand]
    private void Leave()
    {
        _dialogue.End();
        Refresh();
    }

    private void Choose(int index)
    {
        _dialogue.Choose(index);
        Refresh();
    }

    private void Refresh()
    {
        IsOpen = _dialogue.IsInConversation;
        DialogueLine line = _dialogue.CurrentLine;
        if (!IsOpen)
        {
            line = DialogueLine.None;
        }

        Line = _localizer.Get(line.Text);
        Speaker = FindSpeaker(line);
        bool skippable = line.Skippable || line.WaitForInput;
        CanAdvance = IsOpen && !line.Text.IsEmpty && !_dialogue.IsWaitingForChoice && skippable;
        Choices.Clear();
        if (!IsOpen)
        {
            return;
        }

        foreach (DialogueChoice choice in _dialogue.Choices)
        {
            int index = choice.Index;
            string text = _localizer.Get(choice.Text);
            string reason = _localizer.Get(choice.LockedReason);
            DialogueChoiceViewModel option = new(index + 1, text, choice.Available, reason, () => Choose(index));
            Choices.Add(option);
        }
    }

    /// <summary>The speaker's character id without its parents (Character.Innkeeper is "Innkeeper"), the conversation's owner when the line names none.</summary>
    private string FindSpeaker(DialogueLine line)
    {
        Entity speaker = line.SpeakerEntity;
        if (speaker == Entity.Null)
        {
            speaker = _dialogue.Owner;
        }

        if (speaker == Entity.Null || !_world.IsAlive(speaker) || !_world.TryGet(speaker, out DialogueParticipant participant))
        {
            return string.Empty;
        }

        return DisplayNames.Leaf(participant.Speaker);
    }
}
