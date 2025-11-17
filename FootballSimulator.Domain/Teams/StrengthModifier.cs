namespace FootballSimulator.Domain.Teams;

public enum StrengthModifierName
{
    RainyWeather,
    ScorchingHeat,
    CrowdSurge,
    HomeAdvantage,
    TravelFatigue,
    Injuries
}

/// <summary>
/// Describes how a specific contextual event (weather, travel, morale, etc.)
/// adjusts one or more strength factors by a percentage during rating calculations.
/// </summary>
public class StrengthModifier
{
    public StrengthModifier(StrengthModifierName name, IEnumerable<FactorAdjustment> adjustments)
    {
        if (adjustments == null)
        {
            throw new ArgumentNullException(nameof(adjustments));
        }

        var adjustmentsArray = adjustments.ToArray();
        if (adjustmentsArray.Length == 0)
        {
            throw new ArgumentException("A modifier must contain at least one adjustment.", nameof(adjustments));
        }

        if (adjustmentsArray.GroupBy(a => a.Factor).Any(group => group.Count() > 1))
        {
            throw new ArgumentException("Each factor can only appear once per modifier.", nameof(adjustments));
        }

        Name = name;
        Adjustments = adjustmentsArray;
    }

    public StrengthModifierName Name { get; }

    public IReadOnlyCollection<FactorAdjustment> Adjustments { get; }

    public double GetPercentageFor(StrengthFactorName factor) =>
        Adjustments.SingleOrDefault(a => a.Factor == factor).Percentage;

    public override string ToString() => $"{Name} ({string.Join(", ", Adjustments.Select(a => $"{a.Factor}:{a.Percentage:P0}"))})";
}
