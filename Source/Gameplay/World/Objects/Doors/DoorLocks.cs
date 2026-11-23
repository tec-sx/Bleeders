using Arch.Core;

using FlickerVoid.Engine.Interaction.Locks;
using FlickerVoid.Engine.Interaction.Toggles;
using FlickerVoid.Engine.Runtime.Events;

namespace Bleeders.Gameplay.World.Objects.Doors;

using World = Arch.Core.World;

public static class DoorLocks
{
    public static void Unlock(World world, EventBus events, Entity target, Entity instigator, bool opening)
    {
        ref Lockable lockable = ref world.Get<Lockable>(target);
        lockable.Locked = false;

        events.Send(new LockChangedEvent(target, false, instigator));
        
        if (!world.Has<Toggle>(target))
        {
            return;
        }

        ref Toggle toggle = ref world.Get<Toggle>(target);
        toggle.Open = opening;

        events.Send(new ToggledEvent(target, toggle.Open, instigator));
    }
}
