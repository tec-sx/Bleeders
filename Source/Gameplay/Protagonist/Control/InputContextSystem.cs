using Arch.System;

using FlickerVoid.Engine.Cameras.Directing;
using FlickerVoid.Engine.Dialogue.Conversations;
using FlickerVoid.Engine.Gameplay.Tags;
using FlickerVoid.Engine.Platform.Input;
using FlickerVoid.Engine.Runtime.Loop;

namespace Bleeders.Gameplay.Protagonist.Control;

using World = Arch.Core.World;

/// <summary>
/// Holds the input context of what the protagonist is caught up in, so only that situation's actions are read: <see cref="Conversation"/>
/// while it talks and <see cref="Cutscene"/> while a skippable shot plays, each innermost from when it began.
/// </summary>
public sealed class InputContextSystem : BaseSystem<World, GameTime>
{
    public static readonly Tag Conversation = new("Input.Context.Conversation");

    public static readonly Tag Cutscene = new("Input.Context.Cutscene");

    private readonly InputActions _actions;
    private readonly DialogueSystem _dialogue;
    private readonly CameraDirector _director;

    public InputContextSystem(World world, InputActions actions, DialogueSystem dialogue, CameraDirector director)
        : base(world)
    {
        _actions = actions;
        _dialogue = dialogue;
        _director = director;
    }

    public override void Update(in GameTime t)
    {
        Hold(Conversation, _dialogue.IsInConversation);
        Hold(Cutscene, _director.CanSkip);
    }

    /// <summary>Enters the context when it begins and leaves it when it ends, so contexts already held keep their order.</summary>
    private void Hold(Tag context, bool held)
    {
        bool holding = _actions.IsInContext(context);
        if (held && !holding)
        {
            _actions.Enter(context);
        }
        else if (!held && holding)
        {
            _actions.Leave(context);
        }
    }
}
