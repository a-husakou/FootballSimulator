using FootballSimulator.Domain.Matches;
using FootballSimulator.Domain.Simulation;
using FootballSimulator.Domain.Teams;

namespace FootballSimulator.Tests.TestDoubles;

public sealed class StubEventProvider : IMatchModifierEventProvider
{
    private readonly Func<MatchFixture, IReadOnlyCollection<StrengthModifierName>?> resolver;

    public StubEventProvider(Func<MatchFixture, IReadOnlyCollection<StrengthModifierName>?>? resolver = null)
    {
        this.resolver = resolver ?? (_ => null);
    }

    public IReadOnlyCollection<StrengthModifierName>? GetEventsForMatch(MatchFixture fixture) =>
        resolver(fixture);
}

