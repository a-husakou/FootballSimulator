using FootballSimulator.Domain.Matches;
using FootballSimulator.Domain.Simulation.StandingCalculation;

namespace FootballSimulator.Domain.Simulation;

public class GroupSimulationResult
{
    public GroupSimulationResult(
        GroupDefinition group,
        IReadOnlyCollection<MatchResult> matches,
        IReadOnlyCollection<GroupStanding> standings)
    {
        Group = group;
        Matches = matches;
        Standings = standings;
    }

    public GroupDefinition Group { get; }

    public IReadOnlyCollection<MatchResult> Matches { get; }

    public IReadOnlyCollection<GroupStanding> Standings { get; }

    public GroupStanding? FindStandingFor(string teamName) =>
        Standings.FirstOrDefault(s => string.Equals(s.Team.Name, teamName, StringComparison.OrdinalIgnoreCase));
}
