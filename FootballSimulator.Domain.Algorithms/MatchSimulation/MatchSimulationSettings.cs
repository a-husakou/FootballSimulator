namespace FootballSimulator.Domain.Algorithms.MatchSimulation;

public sealed record MatchSimulationSettings
{
    public static MatchSimulationSettings Default { get; } = new();

    /// <summary>
    /// Baseline expected goals when both teams are evenly matched.
    /// </summary>
    public double BaseExpectedGoals { get; init; } = 1.35;

    /// <summary>
    /// How strongly the relative team strength influences the expected goals.
    /// </summary>
    public double StrengthImpact { get; init; } = 1.85;

    /// <summary>
    /// Lower bound for the Poisson lambda to avoid zero-inflation.
    /// </summary>
    public double MinimumLambda { get; init; } = 0.05;

    /// <summary>
    /// Upper bound for the Poisson lambda to keep scorelines reasonable.
    /// </summary>
    public double MaximumLambda { get; init; } = 4.75;
}
