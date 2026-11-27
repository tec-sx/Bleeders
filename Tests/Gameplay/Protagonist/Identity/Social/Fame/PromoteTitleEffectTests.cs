using FlickerVoid.Engine.Gameplay.Conditions;
using FlickerVoid.Engine.Gameplay.Facts;
using FlickerVoid.Engine.Gameplay.Localization;
using FlickerVoid.Engine.Gameplay.Tags;
using Bleeders.Gameplay.Protagonist.Identity.Social.Fame;

namespace Bleeders.Gameplay.Tests.Protagonist.Identity.Social.Fame;

public class PromoteTitleEffectTests
{
    private readonly TitleDefinition _hero = new() { Id = new Tag("Fact.Title.Hero"), Display = new() { Name = new LocalizedText("", "Hero") } };
    private readonly PromoteTitleEffectTestsBuilder _builder;

    public PromoteTitleEffectTests()
    {
        _builder = new PromoteTitleEffectTestsBuilder(_hero);
    }

    [Fact]
    public void PromotesTheTitle()
    {
        PromoteTitleEffect effect = _builder.Build();

        effect.Apply(_builder.Context);

        _builder.Titles.Received(1).PromoteTitle(_hero);
    }

    [Fact]
    public void DoesNothingWithoutTitles()
    {
        IFactDatabase facts = Substitute.For<IFactDatabase>();
        ConditionContext context = new(facts);
        PromoteTitleEffect effect = _builder.Build();

        Action apply = () => effect.Apply(context);

        apply.Should().NotThrow();
    }

    [Fact]
    public void DescribesTheTitle()
    {
        PromoteTitleEffect effect = _builder.Build();

        string text = effect.Describe();

        text.Should().Be("Promote title Hero");
    }
}
