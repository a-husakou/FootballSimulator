using FootballSimulator.Domain.Teams;

namespace FootballSimulator.Domain.Simulation;

public class GroupDefinition
{
    public string Name { get; }

    public IReadOnlyCollection<Team> Teams { get; }

    public GroupDefinition(string name, IEnumerable<Team> teams)
    {
        if (string.IsNullOrWhiteSpace(name))
        {
            throw new ArgumentException("Group name must be provided.", nameof(name));
        }

        if (teams == null)
        {
            throw new ArgumentNullException(nameof(teams));
        }

        var teamArray = teams.ToArray();
        if (teamArray.Length < 2)
        {
            throw new ArgumentException("Not enough teams for group play", nameof(teams));
        }

        if (teamArray.Distinct().Count() != teamArray.Length)
        {
            throw new ArgumentException("Teams in a group must be unique.", nameof(teams));
        }

        Name = name;
        Teams = teamArray;
    }
}
