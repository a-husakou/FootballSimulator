using FootballSimulator.Domain.Teams;

namespace FootballSimulator.Domain.Simulation.StandingCalculation;

public class GroupStanding
{
    public GroupStanding(
        Team team,
        int played,
        int wins,
        int draws,
        int losses,
        int goalsFor,
        int goalsAgainst,
        int points,
        int position)
    {
        Team = team;
        Played = played;
        Wins = wins;
        Draws = draws;
        Losses = losses;
        GoalsFor = goalsFor;
        GoalsAgainst = goalsAgainst;
        Points = points;
        Position = position;
    }

    public Team Team { get; }

    public int Position { get; }

    public int Played { get; }

    public int Wins { get; }

    public int Draws { get; }

    public int Losses { get; }

    public int GoalsFor { get; }

    public int GoalsAgainst { get; }

    public int GoalDifference => GoalsFor - GoalsAgainst;

    public int Points { get; }
}
