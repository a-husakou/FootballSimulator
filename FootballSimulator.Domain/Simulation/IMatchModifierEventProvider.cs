using FootballSimulator.Domain.Matches;
using FootballSimulator.Domain.Teams;

namespace FootballSimulator.Domain.Simulation;

public interface IMatchModifierEventProvider
{
    IReadOnlyCollection<StrengthModifierName>? GetEventsForMatch(MatchFixture fixture);
}
