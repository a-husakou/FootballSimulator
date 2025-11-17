using System.Collections.Immutable;
using FootballSimulator.Domain.Matches;
using FootballSimulator.Domain.Teams;

namespace FootballSimulator.Domain.Simulation;

public class RoundRobinGroupScheduler : IGroupScheduler
{
    public IReadOnlyCollection<MatchFixture> GenerateFixtures(GroupDefinition group)
    {
        if (group == null)
        {
            throw new ArgumentNullException(nameof(group));
        }

        var teams = group.Teams.ToImmutableArray();
        if (teams.Length % 2 != 0)
        {
            throw new ArgumentException("Round robin scheduling requires an even number of teams.");
        }

        var fixtures = new List<MatchFixture>();
        var mutable = teams.ToArray();
        var totalTeams = mutable.Length;
        var rounds = totalTeams - 1;

        for (var round = 0; round < rounds; round++)
        {
            for (var i = 0; i < totalTeams / 2; i++)
            {
                var home = mutable[i];
                var away = mutable[totalTeams - 1 - i];

                if (home == null || away == null)
                {
                    continue;
                }

                if (round % 2 == 1)
                {
                    (home, away) = (away, home);
                }

                fixtures.Add(new MatchFixture(round + 1, home, away));
            }

            Rotate(mutable);
        }

        return fixtures;
    }

    private static void Rotate(Team[] teams)
    {
        if (teams.Length <= 2)
        {
            return;
        }

        var lastIndex = teams.Length - 1;
        var buffer = teams[lastIndex];

        for (var i = lastIndex; i > 1; i--)
        {
            teams[i] = teams[i - 1];
        }

        teams[1] = buffer;
    }
}
