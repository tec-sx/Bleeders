using FlickerVoid.Engine.Gameplay.Localization;

namespace Bleeders.Interface.Localization;

/// <summary>The texts the interface writes in code: its headings, buttons and labels, which <see cref="InterfaceModule"/> declares to the localizer.</summary>
public static class InterfaceTexts
{
    public static LocalizedText Objectives { get; } = new("Hud.Objectives", "OBJECTIVES", "The heading over the tracked quests, top left; shown in capitals.");

    public static LocalizedText Next { get; } = new("Hud.Dialogue.Next", "Next", "The dialogue box's button that moves on to the next line.");

    public static LocalizedText Leave { get; } = new("Hud.Dialogue.Leave", "Leave", "The dialogue box's button that ends the conversation.");

    public static LocalizedText Scan { get; } = new("Hud.Scan.Heading", "SCAN", "The heading of the scan card; shown in capitals.");

    public static LocalizedText ScanMode { get; } = new("Hud.Scan.Mode", "Scan mode", "The scan card's title while nothing is focused.");

    public static LocalizedText Inventory { get; } = new("Hub.Inventory", "Inventory", "The hub's page of carried and worn things.");

    public static LocalizedText Character { get; } = new("Hub.Character", "Character", "The hub's page of attributes, effects and abilities.");

    public static LocalizedText Journal { get; } = new("Hub.Journal", "Journal", "The hub's page of quests and knowledge.");

    public static LocalizedText Weight { get; } = new("Hub.Inventory.Weight", "WEIGHT", "The carried weight's label; shown in capitals.");

    public static LocalizedText Money { get; } = new("Hub.Inventory.Money", "MONEY", "The money's label; shown in capitals.");

    public static LocalizedText Use { get; } = new("Hub.Inventory.Use", "Use", "The button that uses the selected item, such as drinking it.");

    public static LocalizedText Wear { get; } = new("Hub.Inventory.Wear", "Wear", "The button that puts the selected clothing on.");

    public static LocalizedText TakeOff { get; } = new("Hub.Inventory.TakeOff", "Take off", "The button that takes the selected clothing off.");

    public static LocalizedText Head { get; } = new("Hub.Inventory.Slot.Head", "Head", "An outfit slot, for hats.");

    public static LocalizedText UpperBody { get; } = new("Hub.Inventory.Slot.UpperBody", "Upper Body", "An outfit slot, for shirts.");

    public static LocalizedText Coat { get; } = new("Hub.Inventory.Slot.Coat", "Coat", "An outfit slot, for coats and jackets.");

    public static LocalizedText LowerBody { get; } = new("Hub.Inventory.Slot.LowerBody", "Lower Body", "An outfit slot, for trousers.");

    public static LocalizedText Shoes { get; } = new("Hub.Inventory.Slot.Shoes", "Shoes", "An outfit slot.");

    public static LocalizedText Storage { get; } = new("Hub.Inventory.Slot.Storage", "Storage", "An outfit slot, for bags.");

    public static LocalizedText Attributes { get; } = new("Hub.Character.Attributes", "ATTRIBUTES", "A heading of the character page; shown in capitals.");

    public static LocalizedText Effects { get; } = new("Hub.Character.Effects", "EFFECTS", "A heading of the character page, over what changes the attributes, such as being drunk; shown in capitals.");

    public static LocalizedText NoEffects { get; } = new("Hub.Character.NoEffects", "Nothing affects you.", "Shown under Effects when nothing changes the attributes.");

    public static LocalizedText Abilities { get; } = new("Hub.Character.Abilities", "ABILITIES", "A heading of the character page; shown in capitals.");

    public static LocalizedText NoAbilities { get; } = new("Hub.Character.NoAbilities", "No abilities.", "Shown under Abilities when there are none.");

    public static LocalizedText Quests { get; } = new("Hub.Journal.Quests", "QUESTS", "A heading of the journal, over the active quests; shown in capitals.");

    public static LocalizedText NoQuests { get; } = new("Hub.Journal.NoQuests", "No quests.", "Shown under Quests when none is active.");

    public static LocalizedText Finished { get; } = new("Hub.Journal.Finished", "FINISHED", "A heading of the journal, over the completed and failed quests; shown in capitals.");

    public static LocalizedText NothingFinished { get; } = new("Hub.Journal.NothingFinished", "Nothing finished yet.", "Shown under Finished when no quest has ended.");

    public static LocalizedText Completed { get; } = new("Hub.Journal.Completed", "Completed", "Under a finished quest's name: it was done.");

    public static LocalizedText Failed { get; } = new("Hub.Journal.Failed", "Failed", "Under a finished quest's name: it can't be done any more.");

    public static LocalizedText Knowledge { get; } = new("Hub.Journal.Knowledge", "KNOWLEDGE", "A heading of the journal, over what the fox has learned; shown in capitals.");

    public static LocalizedText NothingLearned { get; } = new("Hub.Journal.NothingLearned", "Nothing learned yet.", "Shown under Knowledge when nothing is known.");

    /// <summary>Every text above, as the module declares them.</summary>
    public static IReadOnlyList<LocalizedText> All { get; } =
    [
        Objectives, Next, Leave, Scan, ScanMode, Inventory, Character, Journal, Weight, Money, Use, Wear, TakeOff, Head, UpperBody,
        Coat, LowerBody, Shoes, Storage, Attributes, Effects, NoEffects, Abilities, NoAbilities, Quests, NoQuests, Finished,
        NothingFinished, Completed, Failed, Knowledge, NothingLearned,
    ];
}
