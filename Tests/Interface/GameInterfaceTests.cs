using System.Numerics;

using FlickerVoid.Engine.Gameplay.Tags;

using Bleeders.Interface.Hub;

using Silk.NET.Input;

namespace Bleeders.Interface.Tests;

public sealed class GameInterfaceTests : IDisposable
{
    private static readonly Tag Move = new("Input.Move");

    private readonly GameInterfaceTestsBuilder _builder = new();

    public void Dispose() => _builder.Dispose();

    [Fact]
    public void TheHubPausesTheGameAndOnlyItsActionsAreReadWhileItIsOpen()
    {
        GameInterface gameInterface = _builder.Build();

        _builder.Press(Key.Tab);
        bool paused = _builder.Game.IsPaused;
        _builder.Hold(Key.W);
        Vector2 moveInHub = _builder.Actions.GetValue(Move);
        _builder.Press(Key.E);
        HubPageViewModel pageAfterNext = gameInterface.Hub.SelectedPage;
        _builder.Press(Key.Tab);
        _builder.Hold(Key.W);

        paused.Should().BeTrue();
        moveInHub.Should().Be(Vector2.Zero);
        pageAfterNext.Should().BeSameAs(gameInterface.Hub.Character);
        gameInterface.Hub.IsOpen.Should().BeFalse();
        _builder.Game.IsPaused.Should().BeFalse();
        _builder.Actions.GetValue(Move).Should().Be(new Vector2(0, 1));
    }

    [Fact]
    public void BackClosesTheHubWhenItIsOpenAndOtherwiseTheWindow()
    {
        GameInterface gameInterface = _builder.Build();
        _builder.Press(Key.Tab);

        _builder.Press(Key.Escape);
        bool hubOpen = gameInterface.Hub.IsOpen;
        bool closedWithHub = _builder.WindowClosed;
        _builder.Press(Key.Escape);

        hubOpen.Should().BeFalse();
        closedWithHub.Should().BeFalse();
        _builder.WindowClosed.Should().BeTrue();
    }

    [Fact]
    public void TheHubDoesntOpenDuringAConversation()
    {
        GameInterface gameInterface = _builder.Build();
        _builder.Talk();

        _builder.Press(Key.Tab);

        gameInterface.Hub.IsOpen.Should().BeFalse();
        _builder.Actions.Context.Should().Be(new Tag("Input.Context.Conversation"));
    }
}
