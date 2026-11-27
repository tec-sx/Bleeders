using FlickerVoid.Engine.Runtime.Loop;
using FlickerVoid.Engine.Ui.Stages;
using Bleeders.Interface.Localization;

namespace Bleeders.Interface;

/// <summary>What the interface needs in every game and in the editor: turntables, which the hub's stage spins, and its texts declared to the localizer.</summary>
public sealed class InterfaceModule : IGameModule
{
    public void Add(Game game)
    {
        game.AddTurntables();
        game.Localization.Declare(InterfaceTexts.All);
    }
}
