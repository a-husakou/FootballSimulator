using FootballSimulator.Domain.Configuration;

namespace FootballSimulator.Domain.Teams;

/// <summary>
/// TODO Explain logic behind strength calculation
/// </summary>
public class TeamStrength
{

    public TeamStrength(double baseRating, StrengthFactor[] factors)
        : this(
            baseRating,
            factors ?? Array.Empty<StrengthFactor>(),
            new StrengthModifier[0])
    {
    }

    private TeamStrength(
        double baseRating,
        StrengthFactor[] factors,
        StrengthModifier[] modifiers)
    {
        if (baseRating is < DomainConstants.Teams.Strength.MinRating or > DomainConstants.Teams.Strength.MaxRating)
        {
            throw new ArgumentOutOfRangeException(
                nameof(baseRating),
                $"Base rating must be between {DomainConstants.Teams.Strength.MinRating} and {DomainConstants.Teams.Strength.MaxRating}.");
        }

        BaseRating = baseRating;
        Factors = factors;
        Modifiers = modifiers;
    }

    public double BaseRating { get; }

    public IReadOnlyCollection<StrengthFactor> Factors { get; }

    public IReadOnlyCollection<StrengthModifier> Modifiers { get; }

    /// <summary>
    /// Final rating on a 0-100 scale that blends raw base rating with factor-derived strength adjusted according to modifiers
    /// </summary>
    public double Rating => Factors.Count == 0
        ? BaseRating
        : CalculateCompositeRating();

    public TeamStrength AdjustedBy(params StrengthModifier[] modifiers) =>
        modifiers is { Length: > 0 }
            ? new TeamStrength(BaseRating, Factors.ToArray(), modifiers.AsEnumerable().Concat(Modifiers).ToArray())
            : this;

    private double CalculateCompositeRating()
    {
        var adjustedFactorRating = CalculateRatingFromFactors();

        var weighted = (BaseRating * DomainConstants.Teams.Strength.BaseRatingWeight +
                        adjustedFactorRating * DomainConstants.Teams.Strength.FactorRatingWeight) /
                       (DomainConstants.Teams.Strength.BaseRatingWeight +
                        DomainConstants.Teams.Strength.FactorRatingWeight);
        return Math.Clamp(
            weighted,
            DomainConstants.Teams.Strength.MinRating,
            DomainConstants.Teams.Strength.MaxRating);
    }

    private double CalculateRatingFromFactors()
    {
        var average = Factors
            .Select(ApplyModifiers)
            .DefaultIfEmpty(0d)
            .Average();

        var rating = average * DomainConstants.Teams.Strength.MaxRating;
        return Math.Clamp(
            rating,
            DomainConstants.Teams.Strength.MinRating,
            DomainConstants.Teams.Strength.MaxRating);
    }

    private double ApplyModifiers(StrengthFactor factor)
    {
        if (Modifiers.Count == 0)
        {
            return factor.Value;
        }

        var totalPercentage = Modifiers.Sum(modifier => modifier.GetPercentageFor(factor.Name));

        var adjusted = factor.Value * (1 + totalPercentage);
        return Math.Clamp(
            adjusted,
            DomainConstants.Teams.Strength.NormalizedMinValue,
            DomainConstants.Teams.Strength.NormalizedMaxValue);
    }

    public override string ToString() => $"Rating={Rating:0.##}";
}
