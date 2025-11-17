namespace FootballSimulator.Domain.Teams;

public class Team : IEquatable<Team>
{
    /// <summary>
    /// Immutable aggregate describing a playable team, its baseline strength,
    /// and the set of contextual modifiers it understands how to react to.
    /// </summary>
    /// <param name="name">Display name of the club/nation.</param>
    /// <param name="baseStrength">Base strength profile including factors.</param>
    /// <param name="strengthModifiers">Optional responses to contextual events.</param>
    public Team(
        string name,
        TeamStrength baseStrength,
        IReadOnlyDictionary<StrengthModifierName, StrengthModifier>? strengthModifiers = null)
    {
        if (string.IsNullOrWhiteSpace(name))
        {
            throw new ArgumentException("Team name must be provided.", nameof(name));
        }

        Name = name;
        BaseStrength = baseStrength ?? throw new ArgumentNullException(nameof(baseStrength));
        StrengthModifiers = strengthModifiers ?? new Dictionary<StrengthModifierName, StrengthModifier>();
    }

    public string Name { get; }

    public TeamStrength BaseStrength { get; }

    public IReadOnlyDictionary<StrengthModifierName, StrengthModifier> StrengthModifiers { get; }

    public double CalculateRating(IEnumerable<StrengthModifierName>? events = null)
    {
        if (events is null)
        {
            return BaseStrength.Rating;
        }

        var modifiers = events
            .Select(evt => StrengthModifiers.TryGetValue(evt, out var response) ? response : null)
            .Where(response => response is not null)
            .Cast<StrengthModifier>()
            .ToArray();

        return modifiers.Length == 0
            ? BaseStrength.Rating
            : BaseStrength.AdjustedBy(modifiers).Rating;
    }
    
    public override string ToString() => Name;

    public bool Equals(Team? other)
    {
        if (ReferenceEquals(null, other))
        {
            return false;
        }

        if (ReferenceEquals(this, other))
        {
            return true;
        }

        return StringComparer.Ordinal.Equals(Name, other.Name);
    }

    public override bool Equals(object? obj) => obj is Team other && Equals(other);

    public override int GetHashCode() => StringComparer.Ordinal.GetHashCode(Name);

    public static bool operator ==(Team? left, Team? right) =>
        EqualityComparer<Team>.Default.Equals(left, right);

    public static bool operator !=(Team? left, Team? right) => !(left == right);
}
