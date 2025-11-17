using FootballSimulator.Domain.Matches;
using FootballSimulator.Domain.Simulation;

namespace FootballSimulator.Tests.TestDoubles;

public sealed class StubGroupScheduler : IGroupScheduler
{
    private readonly IReadOnlyList<MatchFixture> fixtures;

    public StubGroupScheduler(IEnumerable<MatchFixture> fixtures)
    {
        if (fixtures == null)
        {
            throw new ArgumentNullException(nameof(fixtures));
        }

        this.fixtures = fixtures.ToList();
    }

    public IReadOnlyCollection<MatchFixture> GenerateFixtures(GroupDefinition group) => fixtures;
}
