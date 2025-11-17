using FootballSimulator.Domain.Matches;

namespace FootballSimulator.Domain.Simulation
{
    public interface IGroupScheduler
    {
        IReadOnlyCollection<MatchFixture> GenerateFixtures(GroupDefinition group);
    }
}
