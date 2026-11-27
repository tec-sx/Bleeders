using CommunityToolkit.Mvvm.ComponentModel;

using FlickerVoid.Engine.Runtime.Localization;

namespace Bleeders.Interface.Localization;

/// <summary>The interface's fixed labels in the chosen language, for views to bind to; they all change when the language does.</summary>
public sealed class InterfaceLabels : ObservableObject
{
    private readonly TextLocalizer _localizer;

    public InterfaceLabels(TextLocalizer localizer)
    {
        _localizer = localizer;
        localizer.Changed += () => OnPropertyChanged(string.Empty);
    }

    public string Objectives => _localizer.Get(InterfaceTexts.Objectives);

    public string Next => _localizer.Get(InterfaceTexts.Next);

    public string Leave => _localizer.Get(InterfaceTexts.Leave);

    public string Scan => _localizer.Get(InterfaceTexts.Scan);

    public string Weight => _localizer.Get(InterfaceTexts.Weight);

    public string Money => _localizer.Get(InterfaceTexts.Money);

    public string Use => _localizer.Get(InterfaceTexts.Use);

    public string Attributes => _localizer.Get(InterfaceTexts.Attributes);

    public string Effects => _localizer.Get(InterfaceTexts.Effects);

    public string NoEffects => _localizer.Get(InterfaceTexts.NoEffects);

    public string Abilities => _localizer.Get(InterfaceTexts.Abilities);

    public string NoAbilities => _localizer.Get(InterfaceTexts.NoAbilities);

    public string Quests => _localizer.Get(InterfaceTexts.Quests);

    public string NoQuests => _localizer.Get(InterfaceTexts.NoQuests);

    public string Finished => _localizer.Get(InterfaceTexts.Finished);

    public string NothingFinished => _localizer.Get(InterfaceTexts.NothingFinished);

    public string Knowledge => _localizer.Get(InterfaceTexts.Knowledge);

    public string NothingLearned => _localizer.Get(InterfaceTexts.NothingLearned);
}
