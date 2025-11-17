using FootballSimulator.Domain.Matches;
using FootballSimulator.Domain.Simulation;
using FootballSimulator.Domain.Teams;
using Xunit;

namespace FootballSimulator.Tests;

public class RoundRobinGroupSchedulerTest
{
    [Fact]
    public void ShouldProduceExpectedFixtureCount()
    {
        var fixtures = GenerateFixtures();
        Assert.Equal(6, fixtures.Count);
    }

    [Fact]
    public void ShouldProduceUniquePairings()
    {
        var fixtures = GenerateFixtures();
        var seenPairs = new HashSet<string>();

        foreach (var fixture in fixtures)
        {
            var pairKey = string.Compare(fixture.HomeTeam.Name, fixture.AwayTeam.Name, StringComparison.Ordinal) < 0
                ? $"{fixture.HomeTeam.Name}-{fixture.AwayTeam.Name}"
                : $"{fixture.AwayTeam.Name}-{fixture.HomeTeam.Name}";

            Assert.True(seenPairs.Add(pairKey), $"Duplicate pairing found for {pairKey}.");
        }
    }

    [Fact]
    public void ShouldAssignRoundsWithinRange()
    {
        var fixtures = GenerateFixtures();
        var totalTeams = 4;

        foreach (var fixture in fixtures)
        {
            Assert.InRange(fixture.Round, 1, totalTeams - 1);
        }

        var rounds = fixtures.GroupBy(f => f.Round).ToList();
        Assert.Equal(totalTeams - 1, rounds.Count);

        foreach (var round in rounds)
        {
            Assert.Equal(totalTeams / 2, round.Count());
            var teamsInRound = new HashSet<string>();
            foreach (var fixture in round)
            {
                Assert.True(teamsInRound.Add(fixture.HomeTeam.Name));
                Assert.True(teamsInRound.Add(fixture.AwayTeam.Name));
            }
        }
    }

    [Fact]
    public void ShouldRequireEvenNumberOfTeams()
    {
        var teams = new[]
        {
            CreateTeam("Alpha"),
            CreateTeam("Bravo"),
            CreateTeam("Charlie")
        };

        var scheduler = new RoundRobinGroupScheduler();
        var group = new GroupDefinition("Odd Group", teams);

        Assert.Throws<ArgumentException>(() => scheduler.GenerateFixtures(group));
    }

    private static List<MatchFixture> GenerateFixtures()
    {
        var teams = new[]
        {
            CreateTeam("Alpha"),
            CreateTeam("Beta"),
            CreateTeam("Gamma"),
            CreateTeam("Delta")
        };

        var scheduler = new RoundRobinGroupScheduler();
        return scheduler.GenerateFixtures(new GroupDefinition("Test Group", teams)).ToList();
    }

    private static Team CreateTeam(string name) =>
        new TeamBuilder()
            .WithName(name)
            .WithBaseRating(60)
            .Build();
}
