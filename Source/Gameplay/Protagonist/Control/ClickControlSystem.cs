using System.Numerics;

using Arch.Core;

using FlickerVoid.Engine.Interaction.Interactors;
using FlickerVoid.Engine.Characters.Movement;
using FlickerVoid.Engine.Dialogue.Conversations;
using FlickerVoid.Engine.Gameplay.Tags;
using FlickerVoid.Engine.Interaction.Interactables;
using FlickerVoid.Engine.Physics;
using FlickerVoid.Engine.Platform.Input;
using FlickerVoid.Engine.Runtime.Components;
using FlickerVoid.Engine.Runtime.Events;

namespace Bleeders.Gameplay.Protagonist.Control;

using World = Arch.Core.World;

/// <summary>
/// Point and click: a press of <see cref="Use"/> walks the protagonist to where the mouse points, or, on an interactable, focuses it,
/// walks within reach and uses it. While the protagonist talks, clicks belong to the conversation and walk nowhere, and while it drives it stays in its seat.
/// </summary>
public sealed class ClickControlSystem
{
    public static readonly Tag Use = new("Input.Use");

    /// <summary>How far into its reach the protagonist walks before using something, so small misses still reach it.</summary>
    public const float ApproachFraction = 0.75f;

    private readonly World _world;
    private readonly EventBus _events;
    private readonly PointerPicker _picker;
    private readonly DialogueSystem _dialogue;
    private Entity _pending = Entity.Null;

    public ClickControlSystem(World world, EventBus events, PointerPicker picker, DialogueSystem dialogue)
    {
        _world = world;
        _events = events;
        _picker = picker;
        _dialogue = dialogue;
        events.Handle((in InputActionEvent action) => HandleInput(action));
        events.Handle((in ArrivedEvent arrived) => HandleArrived(arrived.Character));
    }

    private void HandleInput(in InputActionEvent action)
    {
        if (action.Action != Use || action.Phase != InputPhase.Pressed || _dialogue.IsInConversation)
        {
            return;
        }

        if (!Player.TryFind(_world, out Player player) || !_picker.TryPick(out RayHit hit))
        {
            return;
        }

        Entity protagonist = player.Entity;
        if (_world.Has<Driving>(protagonist))
        {
            return;
        }

        if (_world.Has<ActiveInteractable>(hit.Entity))
        {
            _events.Send(new FocusCommand(protagonist, hit.Entity));
            _pending = hit.Entity;
            Approach(player, hit.Entity);
            return;
        }

        _pending = Entity.Null;
        _events.Send(new FocusCommand(protagonist, Entity.Null));
        _events.Send(new MoveCommand(protagonist, hit.Point));
    }

    private void HandleArrived(Entity character)
    {
        if (_pending == Entity.Null || !Player.Is(_world, character))
        {
            return;
        }

        _pending = Entity.Null;
        _events.Send(new InteractionInput(character, Use, InputPhase.Pressed));
    }

    /// <summary>Walks within reach of the target, or uses it at once when already there.</summary>
    private void Approach(Player player, Entity target)
    {
        Entity protagonist = player.Entity;
        Vector3 position = player.Transform.Position;
        Vector3 targetPosition = InteractionSystem.GetUsePoint(_world, target);
        float reach = player.Interactor.Reach;
        Vector3 away = position - targetPosition;
        away.Y = 0;
        float distance = away.Length();
        float standOff = reach * ApproachFraction;
        if (distance <= standOff)
        {
            _events.Send(new StopCommand(protagonist));
            HandleArrived(protagonist);
            return;
        }

        Vector3 direction = away / distance;
        Vector3 stand = targetPosition + (direction * standOff);
        stand.Y = position.Y;
        _events.Send(new MoveCommand(protagonist, stand));
    }
}
