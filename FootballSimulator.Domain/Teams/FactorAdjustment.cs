using FootballSimulator.Domain.Configuration;

namespace FootballSimulator.Domain.Teams;

public readonly record struct FactorAdjustment
{
    public FactorAdjustment(StrengthFactorName factor, double percentage)
    {
        if (percentage < DomainConstants.Teams.Strength.MinAdjustmentPercentage ||
            percentage > DomainConstants.Teams.Strength.MaxAdjustmentPercentage)
        {
            throw new ArgumentOutOfRangeException(
                nameof(percentage),
                $"Percentage must be between {DomainConstants.Teams.Strength.MinAdjustmentPercentage} and {DomainConstants.Teams.Strength.MaxAdjustmentPercentage}.");
        }

        Factor = factor;
        Percentage = percentage;
    }

    public StrengthFactorName Factor { get; }

    /// <summary>
    /// Percentage expressed as -1..1 (e.g. -0.2 = -20%).
    /// </summary>
    public double Percentage { get; }
}
