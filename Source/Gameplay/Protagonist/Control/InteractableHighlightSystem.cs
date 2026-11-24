using System.Numerics;

using Arch.Core;
using Arch.System;

using FlickerVoid.Engine.Gameplay.Tags;
using FlickerVoid.Engine.Interaction.Interactables;
using FlickerVoid.Engine.Interaction.Interactors;
using FlickerVoid.Engine.Rendering.Outlines;
using FlickerVoid.Engine.Runtime.Components;
using FlickerVoid.Engine.Runtime.Loop;

namespace Bleeders.Gameplay.Protagonist.Control;

using World = Arch.Core.World;

/// <summary>
/// Outlines the interactables an <see cref="InteractableHighlight"/>'s interactor could use within its range and takes the outline off the
/// rest, as an outline request (<see cref="Source"/>), so an interactable's own outline comes back and other requests can outrank it.
/// </summary>
public sealed class InteractableHighlightSystem : BaseSystem<World, GameTime>
{
    /// <summary>The source of the highlight's outline requests.</summary>
    public static readonly Tag Source = new("Outline.Highlight");

    private static readonly QueryDescription Highlighters = new QueryDescription().WithAll<InteractableHighlight, Interactor, Transform>();
    private static readonly QueryDescription Interactables = new QueryDescription().WithAll<ActiveInteractable, Transform>();

    private readonly List<Entity> _lit = [];
    private readonly List<Entity> _unlit = [];

    public InteractableHighlightSystem(World world)
        : base(world)
    {
    }

    public override void Update(in GameTime time)
    {
        bool found = false;
        InteractableHighlight highlight = default;
        Interactor interactor = default;
        Transform origin = default;
        World.Query(in Highlighters, (ref InteractableHighlight settings, ref Interactor settingsInteractor, ref Transform transform) =>
        {
            found = true;
            highlight = settings;
            interactor = settingsInteractor;
            origin = transform;
        });

        float reach = highlight.Range * highlight.Range;
        World.Query(in Interactables, (Entity entity, ref ActiveInteractable active, ref Transform transform) =>
        {
            bool usable = found && active.CanInteract && active.Definition.Accepts(interactor.Kind);
            float distance = Vector3.DistanceSquared(origin.Position, transform.Position);
            bool lit = usable && distance <= reach;
            bool requested = OutlineRequests.IsRequested(World, entity, Source);
            if (lit && !requested)
            {
                _lit.Add(entity);
            }
            else if (!lit && requested)
            {
                _unlit.Add(entity);
            }
        });

        foreach (Entity entity in _lit)
        {
            OutlineRequests.Request(World, entity, Source, highlight.Color);
        }

        foreach (Entity entity in _unlit)
        {
            OutlineRequests.Withdraw(World, entity, Source);
        }

        _lit.Clear();
        _unlit.Clear();
    }
}
