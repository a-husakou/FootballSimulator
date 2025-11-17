namespace FootballSimulator.Domain.Teams;

public class TeamBuilder
{
    private string? name;
    private double baseRating;
    private readonly List<StrengthFactor> factorList = new();
    private readonly Dictionary<StrengthModifierName, List<FactorAdjustment>> responses = new();

    public TeamBuilder WithName(string name)
    {
        this.name = string.IsNullOrWhiteSpace(name)
            ? throw new ArgumentException("Team name must be provided.", nameof(name))
            : name.Trim();
        return this;
    }

    public TeamBuilder WithBaseRating(double baseRating)
    {
        this.baseRating = baseRating;
        return this;
    }

    public TeamBuilder WithFactor(StrengthFactorName name, double value)
    {
        factorList.Add(new StrengthFactor(name, value));
        return this;
    }

    public TeamBuilder RespondsTo(StrengthModifierName modifier, params FactorAdjustment[] adjustments)
    {
        if (!responses.TryGetValue(modifier, out var list))
        {
            list = new List<FactorAdjustment>();
            responses[modifier] = list;
        }

        list.AddRange(adjustments.Where(a => a.Percentage != 0));
        return this;
    }

    public Team Build()
    {
        var strength = new TeamStrength(baseRating, factorList.ToArray());
        var responseMap = responses.ToDictionary(
            kvp => kvp.Key,
            kvp => new StrengthModifier(kvp.Key, kvp.Value));

        return new Team(name, strength, responseMap);
    }
}
