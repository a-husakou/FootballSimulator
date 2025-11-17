using FootballSimulator.Domain.Teams;
using Xunit;

namespace FootballSimulator.Tests;

public class TeamTests
{
    [Theory]
    [MemberData(nameof(ConstructorInvariants))]
    public void ShouldEnforceConstructorInvariants(Type expectedException, Action createTeam)
    {
        Assert.Throws(expectedException, () => createTeam());
    }

    public static IEnumerable<object[]> ConstructorInvariants()
    {
        yield return new object[]
        {
            typeof(ArgumentException),
            (Action)(() => new Team(
                "",
                new TeamStrength(50, Array.Empty<StrengthFactor>())))
        };

        yield return new object[]
        {
            typeof(ArgumentNullException),
            (Action)(() => new Team(
                "Valid Name",
                null!))
        };
    }

    [Fact]
    public void ShouldReturnBaseRating_WhenNoFactorsExistDespiteModifierSensitivity()
    {
        var baseRating = 75.0;

        var builder = new TeamBuilder();
        builder.WithName("Test Team");
        builder.WithBaseRating(baseRating);
        builder.RespondsTo(StrengthModifierName.HomeAdvantage, new FactorAdjustment(StrengthFactorName.Attack, 0.1));

        var sutTeam = builder.Build();

        // Assert
        Assert.Equal(baseRating, sutTeam.CalculateRating(new[] { StrengthModifierName.HomeAdvantage }));
    }

    [Theory]
    [InlineData(null)]
    [InlineData(new[] { StrengthModifierName.ScorchingHeat })]
    public void ShouldBlendBaseAndFactorRatings_WhenFactorsAreDefined(StrengthModifierName[]? activeEvents)
    {
        var baseRating = 50.0;
        var expectedRating = 58.0; // 60% base (50) + 40% factors (70) = 58

        var builder = new TeamBuilder();
        builder.WithName("Factor Test Team");
        builder.WithBaseRating(baseRating);
        builder.WithFactor(StrengthFactorName.Attack, 0.8);
        builder.WithFactor(StrengthFactorName.Defense, 0.6);
        builder.RespondsTo(
            StrengthModifierName.HomeAdvantage,
            new FactorAdjustment(StrengthFactorName.Attack, 0.1));

        var sutTeam = builder.Build();

        Assert.Equal(expectedRating, sutTeam.CalculateRating(activeEvents));
    }

    [Fact]
    public void ShouldReactOnStrengthModifiers()
    {
        var baseRating = 70.0;
        var expectedRating = 67.4; // Composite applying positive and negative modifiers.

        var builder = new TeamBuilder();
        builder.WithName("Multiple Modifiers Team");
        builder.WithBaseRating(baseRating);
        builder.WithFactor(StrengthFactorName.Attack, 0.8);
        builder.WithFactor(StrengthFactorName.Defense, 0.6);
        builder.RespondsTo(
            StrengthModifierName.RainyWeather,
            new FactorAdjustment(StrengthFactorName.Attack, 0.1),
            new FactorAdjustment(StrengthFactorName.Defense, -0.05));
        builder.RespondsTo(
            StrengthModifierName.TravelFatigue,
            new FactorAdjustment(StrengthFactorName.Attack, -0.15),
            new FactorAdjustment(StrengthFactorName.Defense, -0.1));

        var sutTeam = builder.Build();
        var events = new[] { StrengthModifierName.RainyWeather, StrengthModifierName.TravelFatigue };

        Assert.Equal(expectedRating, sutTeam.CalculateRating(events));
    }
}
