using FootballSimulator.Domain;
using FootballSimulator.Domain.Algorithms.MatchSimulation;
using FootballSimulator.Domain.Matches;
using FootballSimulator.Domain.Teams;
using Xunit;

namespace FootballSimulator.Tests;

public class KnuthAlgorithmMatchSimulationTests
{
    [Fact]
    public void StrongerTeamShouldOutperformWeakerTeam()
    {
        var homeTeam = CreateTeam(
            "Dominant XI",
            baseRating: 78,
            (StrengthFactorName.Attack, 0.88),
            (StrengthFactorName.MidfieldControl, 0.82),
            (StrengthFactorName.Defense, 0.8));

        var awayTeam = CreateTeam(
            "Underdogs",
            baseRating: 66,
            (StrengthFactorName.Attack, 0.64),
            (StrengthFactorName.MidfieldControl, 0.58),
            (StrengthFactorName.Defense, 0.6));

        var fixture = new MatchFixture(1, homeTeam, awayTeam);
        var simulator = new KnuthAlgorithmMatchSimulation(new SeededRandomProvider(1234));

        var iterations = 500;
        var homeRating = homeTeam.CalculateRating();
        var awayRating = awayTeam.CalculateRating();

        var homeWins = 0;
        var awayWins = 0;
        var draws = 0;
        var cumulativeGoalDifference = 0;

        for (var i = 0; i < iterations; i++)
        {
            var match = simulator.Simulate(fixture, homeRating, awayRating);
            var homeGoals = match.Score.GetGoalsFor(homeTeam);
            var awayGoals = match.Score.GetGoalsFor(awayTeam);

            cumulativeGoalDifference += homeGoals - awayGoals;

            if (homeGoals > awayGoals)
            {
                homeWins++;
            }
            else if (awayGoals > homeGoals)
            {
                awayWins++;
            }
            else
            {
                draws++;
            }
        }

        Assert.True(homeWins > awayWins, "Stronger team should secure more wins across many simulations.");
        Assert.True(cumulativeGoalDifference > 0, "Average goal difference should favor stronger team.");
        Assert.True(homeWins > draws, "Wins for stronger side should exceed draws in long-run simulation.");
    }

    private static Team CreateTeam(string name, double baseRating, params (StrengthFactorName Factor, double Value)[] factors)
    {
        var builder = new TeamBuilder()
            .WithName(name)
            .WithBaseRating(baseRating);

        foreach (var (factor, value) in factors)
        {
            builder.WithFactor(factor, value);
        }

        return builder.Build();
    }

    private class SeededRandomProvider : IRandomProvider
    {
        private readonly Random random;

        public SeededRandomProvider(int seed)
        {
            random = new Random(seed);
        }

        public double NextDouble() => random.NextDouble();
    }
}
