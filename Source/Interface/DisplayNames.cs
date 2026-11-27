using FlickerVoid.Engine.Gameplay.Definitions;
using FlickerVoid.Engine.Gameplay.Tags;
using FlickerVoid.Engine.Runtime.Localization;

namespace Bleeders.Interface;

/// <summary>The names the interface shows for tags and definitions that may have no display name.</summary>
public static class DisplayNames
{
    /// <summary>The tag without its parents: Character.Innkeeper is "Innkeeper".</summary>
    public static string Leaf(Tag tag)
    {
        string name = tag.Name;
        int last = name.LastIndexOf('.');
        return name[(last + 1)..];
    }

    /// <summary>The definition's display name in the chosen language, else the leaf of its id.</summary>
    public static string Of(Definition definition, TextLocalizer localizer)
    {
        if (!definition.Display.Name.IsEmpty)
        {
            return localizer.Get(definition.Display.Name);
        }

        return Leaf(definition.Id);
    }
}
