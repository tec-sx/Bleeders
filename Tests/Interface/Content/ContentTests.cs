using FlickerVoid.Engine.Runtime.Content;

namespace Bleeders.Interface.Tests.Content;

public sealed class ContentTests : IDisposable
{
    private readonly ContentTestsBuilder _builder = new();

    public void Dispose() => _builder.Dispose();

    [Fact]
    public void EveryContentFileReadsAndWhatItNamesIsThere()
    {
        IReadOnlyList<ContentProblem> problems = _builder.Check();

        problems.Should().BeEmpty();
    }
}
