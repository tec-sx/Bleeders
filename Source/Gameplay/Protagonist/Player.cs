using Arch.Core;

using FlickerVoid.Engine.Characters.Movement;
using FlickerVoid.Engine.Interaction.Interactors;
using FlickerVoid.Engine.Runtime.Components;
using FlickerVoid.Engine.Runtime.Entities;

namespace Bleeders.Gameplay.Protagonist;

using World = Arch.Core.World;

/// <summary>The protagonist the player moves: its marker, its interactor, how it walks and where it stands.</summary>
[EntityClass]
public readonly partial struct Player
{
    public partial ref Protagonist Protagonist { get; }

    public partial ref Interactor Interactor { get; }

    public partial ref CharacterMover Mover { get; }

    public partial ref Transform Transform { get; }

    /// <summary>The first player in the world; false when there is none.</summary>
    public static bool TryFind(World world, out Player player)
    {
        Entity found = Entity.Null;
        QueryDescription players = Query;
        world.Query(in players, (Entity entity) =>
        {
            if (found == Entity.Null)
            {
                found = entity;
            }
        });
        player = default;
        if (found == Entity.Null)
        {
            return false;
        }

        player = From(world, found);
        return true;
    }
}
