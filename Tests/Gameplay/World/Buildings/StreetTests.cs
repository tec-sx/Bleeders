using System.Numerics;

namespace Bleeders.Gameplay.Tests.World.Buildings;

public sealed class StreetTests : IDisposable
{
    private readonly StreetTestsBuilder _builder = new();

    public StreetTests()
    {
        _builder.Start();
    }

    public void Dispose() => _builder.Dispose();

    [Fact]
    public void OpensTheInnWhileTheFoxIsInside()
    {
        _builder.Story.Run(0.1f);

        _builder.Cutaways.Cutaway.Should().Be(_builder.Story.Find("Inn Ground"));
        _builder.IsShadowOnly("Inn Roof").Should().BeTrue();
        _builder.IsShadowOnly("Inn Upper Front").Should().BeTrue();
    }

    [Fact]
    public void FollowsTheFoxDownTheStreet()
    {
        bool arrived = _builder.WalkTo(new Vector3(8, 0, 6));

        arrived.Should().BeTrue();
        _builder.Cutaways.Cutaway.Should().BeNull();
        _builder.IsShadowOnly("Inn Roof").Should().BeFalse();
        _builder.Director.Live.Should().Be(_builder.Story.Find("Street"));
    }

    [Fact]
    public void WalksUpToTheHousesUpperFloor()
    {
        bool arrived = _builder.WalkTo(new Vector3(15, 3.2f, 1));

        arrived.Should().BeTrue();
        _builder.Cutaways.Cutaway.Should().Be(_builder.Story.Find("House Upper"));
        _builder.IsShadowOnly("House Roof").Should().BeTrue();
        _builder.IsShadowOnly("House Ceiling").Should().BeFalse("the fox stands on it");
    }

    [Fact]
    public void WalksDownIntoTheWineCellar()
    {
        bool arrived = _builder.WalkTo(new Vector3(-13, -3, 1));

        arrived.Should().BeTrue();
        _builder.Cutaways.Cutaway.Should().Be(_builder.Story.Find("Wine Cellar"));
        _builder.IsShadowOnly("Shack Floor East").Should().BeTrue();
        _builder.IsShadowOnly("Shack Roof East").Should().BeTrue();
        _builder.IsShadowOnly("Barrel 1").Should().BeFalse();
    }

    [Fact]
    public void KeepsTheInnOpenOnceTheFoxStops()
    {
        _builder.Steer(-Vector3.UnitX, 1);

        _builder.Cutaways.Cutaway.Should().Be(_builder.Story.Find("Inn Ground"));
    }

    [Fact]
    public void KeepsTheFoxInsideTheInnsWalls()
    {
        _builder.Steer(Vector3.UnitX, 4);

        Vector3 position = _builder.GetPosition(_builder.Story.Fox);
        position.X.Should().BeLessThan(4, "the inn's right wall stops it");
        position.Y.Should().BeApproximately(0, 0.05f, "it stays on the floor");
    }
}
