using System;
using FootballSimulator.Domain.Algorithms;
using FootballSimulator.Domain.Algorithms.MatchSimulation;
using FootballSimulator.Domain.Simulation;
using FootballSimulator.Domain.Teams;

namespace FootballSimulator.Application;

/// <summary>
/// Picks the teams and runs simulation
/// </summary>
public class RandomGroupSimulationRunner
{
    private readonly IGroupSimulator groupSimulator;

    public RandomGroupSimulationRunner(int? seed = null)
        : this(new DefaultGroupSimulator(
            new KnuthAlgorithmMatchSimulation(new SystemRandomProvider(seed)),
            new RoundRobinGroupScheduler(),
            new RandomMatchModifierEventProvider(new SystemRandomProvider(seed)))
        )
    { }

    public RandomGroupSimulationRunner(IGroupSimulator groupSimulator)
    {
        this.groupSimulator = groupSimulator ?? throw new ArgumentNullException(nameof(groupSimulator));
    }

    public GroupSimulationResult RunOnce()
    {
        var teams = CreateTeams();
        var group = new GroupDefinition("MiniClip Assignment Simulation", teams);

        return groupSimulator.Simulate(group);
    }

    private static Func<int?, IGroupSimulator> CreateDefaultSimulatorFactory()
    {
        return seed =>
        {
            var random = new SystemRandomProvider(seed);
            var matchSimulator = new KnuthAlgorithmMatchSimulation(random);
            var groupScheduler = new RoundRobinGroupScheduler();
            var eventProvider = new RandomMatchModifierEventProvider(random);
            return new DefaultGroupSimulator(matchSimulator, groupScheduler, eventProvider);
        };
    }

    // hardcoding for this assignment as no specific instructions are given, in real app Teams will come from some store and there will likely be some Teams selection logic.
    private static IReadOnlyCollection<Team> CreateTeams()
    {
        return new[]
        {
            new TeamBuilder()
                .WithName("Coastal Mariners")
                .WithFactor(StrengthFactorName.Attack, 0.78)
                .WithFactor(StrengthFactorName.MidfieldControl, 0.74)
                .WithFactor(StrengthFactorName.TeamSpirit, 0.82)
                .WithFactor(StrengthFactorName.Defense, 0.68)
                .RespondsTo(
                    StrengthModifierName.HomeAdvantage,
                    new FactorAdjustment(StrengthFactorName.Attack, 0.03),
                    new FactorAdjustment(StrengthFactorName.TeamSpirit, 0.05))
                .RespondsTo(
                    StrengthModifierName.RainyWeather,
                    new FactorAdjustment(StrengthFactorName.Attack, 0.05),
                    new FactorAdjustment(StrengthFactorName.TeamSpirit, 0.1))
                .RespondsTo(
                    StrengthModifierName.ScorchingHeat,
                    new FactorAdjustment(StrengthFactorName.Stamina, -0.2),
                    new FactorAdjustment(StrengthFactorName.Attack, -0.1))
                .Build(),
            new TeamBuilder()
                .WithName("Mountain Rangers")
                .WithFactor(StrengthFactorName.Defense, 0.81)
                .WithFactor(StrengthFactorName.Stamina, 0.77)
                .WithFactor(StrengthFactorName.Goalkeeping, 0.75)
                .WithFactor(StrengthFactorName.TeamSpirit, 0.71)
                .RespondsTo(
                    StrengthModifierName.HomeAdvantage,
                    new FactorAdjustment(StrengthFactorName.Attack, 0.03),
                    new FactorAdjustment(StrengthFactorName.TeamSpirit, 0.05))
                .RespondsTo(
                    StrengthModifierName.TravelFatigue,
                    new FactorAdjustment(StrengthFactorName.Attack, -0.05),
                    new FactorAdjustment(StrengthFactorName.TeamSpirit, -0.03))
                .Build(),
            new TeamBuilder()
                .WithName("Metro Strikers")
                .WithFactor(StrengthFactorName.Attack, 0.82)
                .WithFactor(StrengthFactorName.MidfieldControl, 0.76)
                .RespondsTo(
                    StrengthModifierName.HomeAdvantage,
                    new FactorAdjustment(StrengthFactorName.Attack, 0.03),
                    new FactorAdjustment(StrengthFactorName.TeamSpirit, 0.05))
                .RespondsTo(
                    StrengthModifierName.CrowdSurge,
                    new FactorAdjustment(StrengthFactorName.TeamSpirit, 0.18),
                    new FactorAdjustment(StrengthFactorName.Attack, 0.1))
                .RespondsTo(
                    StrengthModifierName.TravelFatigue,
                    new FactorAdjustment(StrengthFactorName.Attack, -0.12))
                .Build(),
            new TeamBuilder()
                .WithName("Desert Falcons")
                .WithFactor(StrengthFactorName.Defense, 0.72)
                .WithFactor(StrengthFactorName.Stamina, 0.74)
                .WithFactor(StrengthFactorName.TeamSpirit, 0.7)
                .WithFactor(StrengthFactorName.Goalkeeping, 0.69)
                .RespondsTo(
                    StrengthModifierName.HomeAdvantage,
                    new FactorAdjustment(StrengthFactorName.Attack, 0.03),
                    new FactorAdjustment(StrengthFactorName.TeamSpirit, 0.05))
                .RespondsTo(
                    StrengthModifierName.ScorchingHeat,
                    new FactorAdjustment(StrengthFactorName.Stamina, 0.15),
                    new FactorAdjustment(StrengthFactorName.TeamSpirit, 0.1))
                .RespondsTo(
                    StrengthModifierName.RainyWeather,
                    new FactorAdjustment(StrengthFactorName.Attack, -0.15),
                    new FactorAdjustment(StrengthFactorName.TeamSpirit, -0.1))
                .Build()
        };
    }
}
