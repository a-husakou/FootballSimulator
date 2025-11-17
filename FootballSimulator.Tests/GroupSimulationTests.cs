using FootballSimulator.Domain.Matches;
using FootballSimulator.Domain.Simulation;
using FootballSimulator.Domain.Teams;
using FootballSimulator.Tests.TestDoubles;
using Moq;
using Xunit;

namespace FootballSimulator.Tests;

public class GroupSimulationTests
{
    [Fact]
    public void ShouldRankTeamsBasedOnTheResult()
    {
        var alpha = CreateTeam("Alpha");
        var beta = CreateTeam("Beta");
        var gamma = CreateTeam("Gamma");
        var delta = CreateTeam("Delta");

        var fixtures = new[]
        {
            new MatchFixture(1, alpha, beta),
            new MatchFixture(2, gamma, alpha),
            new MatchFixture(3, alpha, delta),
            new MatchFixture(4, beta, gamma),
            new MatchFixture(5, beta, delta),
            new MatchFixture(6, gamma, delta)
        };

        var scripts = new Dictionary<(string Home, string Away), (int HomeGoals, int AwayGoals)>
        {
            { (alpha.Name, beta.Name), (2, 1) },
            { (gamma.Name, alpha.Name), (1, 1) },
            { (alpha.Name, delta.Name), (3, 0) },
            { (beta.Name, gamma.Name), (2, 1) },
            { (beta.Name, delta.Name), (1, 1) },
            { (gamma.Name, delta.Name), (2, 1) }
        };

        var simulator = new StubMatchSimulator(scripts);
        var scheduler = new StubGroupScheduler(fixtures);
        var sut = new DefaultGroupSimulator(simulator, scheduler, new StubEventProvider());
        var group = new GroupDefinition("Match Group", new[] { alpha, beta, gamma, delta });

        var result = sut.Simulate(group);

        Assert.Collection(
            result.Standings,
            standing =>
            {
                Assert.Equal("Alpha", standing.Team.Name);
                Assert.Equal(7, standing.Points);
                Assert.Equal(6, standing.GoalsFor);
                Assert.Equal(2, standing.GoalsAgainst);
            },
            standing =>
            {
                Assert.Equal("Beta", standing.Team.Name);
                Assert.Equal(4, standing.Points);
                Assert.Equal(4, standing.GoalsFor);
                Assert.Equal(4, standing.GoalsAgainst);
            },
            standing =>
            {
                Assert.Equal("Gamma", standing.Team.Name);
                Assert.Equal(4, standing.Points);
                Assert.Equal(4, standing.GoalsFor);
                Assert.Equal(4, standing.GoalsAgainst);
            },
            standing =>
            {
                Assert.Equal("Delta", standing.Team.Name);
                Assert.Equal(1, standing.Points);
                Assert.Equal(2, standing.GoalsFor);
                Assert.Equal(6, standing.GoalsAgainst);
            });
    }

    [Fact]
    public void ShouldApplyEventsBeforeCalculatingRatings()
    {
        var alpha = new TeamBuilder()
            .WithName("Alpha")
            .WithBaseRating(60)
            .WithFactor(StrengthFactorName.Attack, 1.0)
            .Build();

        var beta = new TeamBuilder()
            .WithName("Beta")
            .WithBaseRating(60)
            .WithFactor(StrengthFactorName.Attack, 1.0)
            .RespondsTo(
                StrengthModifierName.TravelFatigue,
                new FactorAdjustment(StrengthFactorName.Attack, -0.3))
            .Build();

        var fixtures = new[]
        {
            new MatchFixture(1, alpha, beta),
            new MatchFixture(2, beta, alpha)
        };

        var scheduler = new StubGroupScheduler(fixtures);
        var eventProvider = new StubEventProvider(fixture =>
            fixture.HomeTeam == beta ? new[] { StrengthModifierName.TravelFatigue } : null);

        var matchSimulator = new Mock<IMatchSimulator>();
        matchSimulator
            .Setup(sim => sim.Simulate(It.IsAny<MatchFixture>(), It.IsAny<double>(), It.IsAny<double>()))
            .Returns<MatchFixture, double, double>((fixture, _, _) =>
                new MatchResult(fixture, new ScoreLine(fixture, 0, 0)));

        var group = new GroupDefinition("Events Group", new[] { alpha, beta });
        var sut = new DefaultGroupSimulator(matchSimulator.Object, scheduler, eventProvider);

        sut.Simulate(group);

        var alphaHomeExpected = alpha.CalculateRating(new[] { StrengthModifierName.HomeAdvantage });
        var betaBaseRating = beta.CalculateRating();
        var betaHomeFatigued = beta.CalculateRating(new[]
        {
            StrengthModifierName.HomeAdvantage,
            StrengthModifierName.TravelFatigue
        });
        var alphaAwayRating = alpha.CalculateRating();

        matchSimulator.Verify(sim => sim.Simulate(
                It.Is<MatchFixture>(f => f.HomeTeam == alpha && f.AwayTeam == beta),
                It.Is<double>(rating => rating == alphaHomeExpected),
                It.Is<double>(rating => rating == betaBaseRating)),
            Times.Once);

        matchSimulator.Verify(sim => sim.Simulate(
                It.Is<MatchFixture>(f => f.HomeTeam == beta && f.AwayTeam == alpha),
                It.Is<double>(rating => rating == betaHomeFatigued),
                It.Is<double>(rating => rating == alphaAwayRating)),
            Times.Once);
    }

    [Theory]
    [MemberData(nameof(StandingTieBreakerScenarios))]
    public void StandingComparerShouldApplyTieBreakersInOrder(
        (string Home, string Away, int HomeGoals, int AwayGoals)[] matches,
        string[] expectedOrder)
    {
        var result = RunScenario(matches);
        Assert.Equal(expectedOrder, result.Standings.Select(s => s.Team.Name).ToArray());
    }

    private static Team CreateTeam(string name) =>
        new TeamBuilder()
            .WithName(name)
            .WithBaseRating(60)
            .Build();

    public static IEnumerable<object[]> StandingTieBreakerScenarios()
    {
        yield return new object[]
        {
            new[]
            {
                ("Alpha", "Bravo", 0, 1),
                ("Bravo", "Charlie", 0, 3),
                ("Charlie", "Alpha", 0, 2)
            },
            new[] { "Charlie", "Alpha", "Bravo" } // goal difference
        };

        yield return new object[]
        {
            new[]
            {
                ("Alpha", "Bravo", 1, 0),
                ("Bravo", "Charlie", 2, 1),
                ("Charlie", "Alpha", 1, 0)
            },
            new[] { "Bravo", "Charlie", "Alpha" } // goals for
        };

        yield return new object[]
        {
            new[]
            {
                ("Alpha", "Beta", 1, 0),
                ("Delta", "Charlie", 0, 1),
                ("Beta", "Charlie", 1, 0),
                ("Delta", "Alpha", 0, 1),
                ("Beta", "Delta", 0, 1),
                ("Charlie", "Alpha", 0, 1)
            },
            new[] { "Alpha", "Beta", "Charlie", "Delta" } // head-to-head
        };
    }

    private static GroupSimulationResult RunScenario(
        IReadOnlyList<(string Home, string Away, int HomeGoals, int AwayGoals)> input)
    {
        var teams = input
            .SelectMany(m => new[] { m.Home, m.Away })
            .Distinct(StringComparer.Ordinal)
            .OrderBy(name => name, StringComparer.Ordinal)
            .Select(CreateTeam)
            .ToDictionary(team => team.Name, team => team, StringComparer.Ordinal);

        var fixtures = input
            .Select((m, index) => new MatchFixture(index + 1, teams[m.Home], teams[m.Away]))
            .ToArray();

        var matches = input.ToDictionary(
            m => (m.Home, m.Away),
            m => (m.HomeGoals, m.AwayGoals));

        var scheduler = new StubGroupScheduler(fixtures);
        var simulator = new StubMatchSimulator(matches);
        var sut = new DefaultGroupSimulator(simulator, scheduler, new StubEventProvider());
        var group = new GroupDefinition("Test Group", teams.Values);

        return sut.Simulate(group);
    }

}
