using Arch.Core;

using FlickerVoid.Engine.Abilities.Activation;
using FlickerVoid.Engine.Attributes.Checks;
using FlickerVoid.Engine.Gameplay.Conditions;
using FlickerVoid.Engine.Gameplay.Facts;
using FlickerVoid.Engine.Gameplay.Tags;
using FlickerVoid.Engine.Interaction.Locks;
using FlickerVoid.Engine.Interaction.Toggles;
using FlickerVoid.Engine.Runtime.Data;
using FlickerVoid.Engine.Runtime.Entities;
using FlickerVoid.Engine.Runtime.Events;
using FlickerVoid.Engine.Runtime.Loop;
using FlickerVoid.Engine.Runtime.Services;
using Bleeders.Gameplay.Serialization;

namespace Bleeders.Gameplay.World.Objects.Doors;

using World = Arch.Core.World;

/// <summary>
/// The lockpick ability, in place of the Unreal lockpick minigame: a pickable lock that a key didn't open is picked by whoever was
/// blocked when they can use <c>Ability.Lockpick</c> (carrying a lockpick); the ability rolls the lockpick check against the lock's
/// difficulty, and a success opens the lock and lets the toggle go ahead, while a failure cancels it and the lock, remembering the
/// picker's bonus (<see cref="FailedPick"/>, saved), is not tried again until a picker brings a higher one.
/// </summary>
public sealed class LockpickSystem
{
    public static readonly Tag Ability = new("Ability.Lockpick");

    public const string CheckPath = "Checks/Lockpick.fvdata";

    /// <summary>The check's difficulty for a lock of difficulty 0; each tenth of the lock's difficulty adds one.</summary>
    public const int BaseDifficulty = 4;

    private readonly World _world;
    private readonly EventBus _events;
    private readonly AbilitySystem _abilities;
    private readonly CheckSystem _checks;
    private readonly IFactDatabase _facts;
    private readonly IServiceRegistry _services;
    private readonly Dictionary<Entity, bool> _opening = [];

    public LockpickSystem(Game game, AbilitySystem abilities, CheckSystem checks)
    {
        _world = game.World;
        _events = game.Events;
        _abilities = abilities;
        _checks = checks;
        _facts = game.Facts;
        _services = game.Services;
        ArrayRegistry.Add<FailedPick>();
        game.Events.Handle((in ToggleBlockedEvent blocked) => TryPick(blocked));
        game.Events.Handle((in AbilityActivatedEvent activated) => Pick(activated));
        game.SaveStates.AddComponent("failedPick", BleedersJsonContext.Default.FailedPick);
    }

    /// <summary>The check's difficulty for a lock of difficulty 0 to 1.</summary>
    public static int GetDifficulty(Lockable target) => BaseDifficulty + (int)MathF.Round(target.Difficulty * 10, MidpointRounding.AwayFromZero);

    private void TryPick(in ToggleBlockedEvent blocked)
    {
        Entity door = blocked.Target;
        if (!_world.IsAlive(door) || !_world.TryGet(door, out Lockable target) || !target.Locked || !target.Pickable)
        {
            return;
        }

        if (!_abilities.CanActivate(blocked.Instigator, Ability, out _, door) || !IsBetterThanLastFailure(blocked.Instigator, door, target))
        {
            return;
        }

        _opening[blocked.Instigator] = blocked.Opening;
        _abilities.TryActivate(blocked.Instigator, Ability, door);
    }

    private void Pick(in AbilityActivatedEvent activated)
    {
        if (activated.Ability.Id != Ability)
        {
            return;
        }

        Entity picker = activated.Owner;
        Entity door = activated.Target;
        CheckDefinition? check = LoadCheck();
        if (check is null || !_world.IsAlive(door) || !_world.TryGet(door, out Lockable target) || !target.Locked)
        {
            _abilities.Cancel(picker, Ability);
            return;
        }

        ConditionContext context = CreateContext(picker, door);
        int difficulty = GetDifficulty(target);
        CheckResult bonus = _checks.Preview(check, context, difficulty);
        CheckResult result = _checks.Roll(check, context, difficulty);
        _events.Send(new LockpickedEvent(picker, door, result));
        if (!result.Success)
        {
            FailedPick failed = new() { Bonus = bonus.Total };
            if (_world.Has<FailedPick>(door))
            {
                _world.Set(door, failed);
            }
            else
            {
                _world.Add(door, failed);
            }

            _abilities.Cancel(picker, Ability);
            return;
        }

        _opening.Remove(picker, out bool opening);
        DoorLocks.Unlock(_world, _events, door, picker, opening);
        _abilities.End(picker, Ability);
    }

    /// <summary>Whether the picker's bonus is higher than when the lock last beat a pick, or it never did.</summary>
    private bool IsBetterThanLastFailure(Entity picker, Entity door, Lockable target)
    {
        if (!_world.TryGet(door, out FailedPick failed))
        {
            return true;
        }

        CheckDefinition? check = LoadCheck();
        if (check is null)
        {
            return false;
        }

        ConditionContext context = CreateContext(picker, door);
        int difficulty = GetDifficulty(target);
        CheckResult bonus = _checks.Preview(check, context, difficulty);
        return bonus.Total > failed.Bonus;
    }

    private ConditionContext CreateContext(Entity picker, Entity door)
    {
        WorldEntityView pickerView = new(_world, picker);
        WorldEntityView doorView = new(_world, door);
        return new ConditionContext(_facts) { Instigator = pickerView, Target = doorView, Services = _services };
    }

    private CheckDefinition? LoadCheck()
    {
        DataAssets? data = _services.GetService<DataAssets>();
        if (data is null)
        {
            return null;
        }

        try
        {
            return data.Load<CheckDefinition>(CheckPath);
        }
        catch (Exception exception) when (exception is InvalidDataException or IOException)
        {
            return null;
        }
    }
}
