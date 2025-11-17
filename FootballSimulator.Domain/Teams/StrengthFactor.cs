using FootballSimulator.Domain.Configuration;

namespace FootballSimulator.Domain.Teams;

public enum StrengthFactorName
{
    Attack,
    Defense,
    MidfieldControl,
    TeamSpirit,
    Goalkeeping,
    Stamina
}

public readonly record struct StrengthFactor
{
    public StrengthFactor(StrengthFactorName name, double value)
    {
        if (value is < DomainConstants.Teams.Strength.NormalizedMinValue or > DomainConstants.Teams.Strength.NormalizedMaxValue)
        {
            throw new ArgumentOutOfRangeException(
                nameof(value),
                $"Factor value must be between {DomainConstants.Teams.Strength.NormalizedMinValue} and {DomainConstants.Teams.Strength.NormalizedMaxValue}.");
        }

        Name = name;
        Value = value;
    }

    public StrengthFactorName Name { get; }

    /// <summary>
    /// Normalized score for this factor (0 = weakest, 1 = strongest).
    /// </summary>
    public double Value { get; }
}
