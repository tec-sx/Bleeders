using Arch.Core;

using FlickerVoid.Engine.Audio.Sounds;

namespace Bleeders.Gameplay.Tests.World.Objects.Doors;

public sealed class DoorSwingSystemTests : IDisposable
{
    private const string Sounds = "\"rattleSound\": \"Sounds/Rattle.fvsound\", \"swingSound\": \"Sounds/Swing.fvsound\"";

    private readonly DoorSwingSystemTestsBuilder _builder = new();

    public void Dispose() => _builder.Dispose();

    [Fact]
    public void ABlockedDoorPlaysItsRattleOnItself()
    {
        Entity door = _builder.SpawnDoor(Sounds);

        _builder.Block(door);

        PlaySoundCommand expected = new("Sounds/Rattle.fvsound", Entity: door);
        _builder.Sounds.Should().ContainSingle().Which.Should().Be(expected);
    }

    [Theory]
    [InlineData(true)]
    [InlineData(false)]
    public void ADoorPlaysItsSwingAsItStartsToOpenOrShut(bool open)
    {
        Entity door = _builder.SpawnDoor(Sounds);

        _builder.Toggle(door, open);

        PlaySoundCommand expected = new("Sounds/Swing.fvsound", Entity: door);
        _builder.Sounds.Should().ContainSingle().Which.Should().Be(expected);
    }

    [Fact]
    public void ADoorThatNamesNoSoundsAsksForNone()
    {
        Entity door = _builder.SpawnDoor("\"openAngle\": 90");

        _builder.Block(door);
        _builder.Toggle(door, true);

        _builder.Sounds.Should().BeEmpty();
    }
}
