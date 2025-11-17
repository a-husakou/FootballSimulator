using FootballSimulator.Domain.Teams;

namespace FootballSimulator.Domain.Matches;

public class ScoreLine
{
    private readonly TeamScore[] scores;

    public ScoreLine(MatchFixture fixture, int homeGoals, int awayGoals)
    {
        if (fixture == null)
        {
            throw new ArgumentNullException(nameof(fixture));
        }

        scores = new[] { new TeamScore(fixture.HomeTeam, homeGoals), new TeamScore(fixture.AwayTeam, awayGoals) };
    }

    public bool IsDraw => scores[0].Goals == scores[1].Goals;

    public bool InvolvesTeam(Team team)
    {
        if (team == null)
        {
            throw new ArgumentNullException(nameof(team));
        }

        foreach (var score in scores)
        {
            if (score.Team.Equals(team))
            {
                return true;
            }
        }

        return false;
    }

    public TeamScore GetScoreFor(Team team) => FindScore(team);

    public TeamScore GetOpponentScore(Team team)
    {
        if (team == null)
        {
            throw new ArgumentNullException(nameof(team));
        }

        if (scores[0].Team.Equals(team))
        {
            return scores[1];
        }

        if (scores[1].Team.Equals(team))
        {
            return scores[0];
        }

        throw new ArgumentException("Team did not participate in this match.", nameof(team));
    }

    public int GetGoalsFor(Team team) => GetScoreFor(team).Goals;

    public int GetGoalsAgainst(Team team) => GetOpponentScore(team).Goals;

    public Team? GetWinner()
    {
        if (IsDraw)
        {
            return null;
        }

        return scores[0].Goals > scores[1].Goals ? scores[0].Team : scores[1].Team;
    }

    public Team? GetLoser()
    {
        if (IsDraw)
        {
            return null;
        }

        return scores[0].Goals > scores[1].Goals ? scores[1].Team : scores[0].Team;
    }

    public Team GetOpponent(Team team) => GetOpponentScore(team).Team;

    public override string ToString() => $"{scores[0].Goals} - {scores[1].Goals}";

    private TeamScore FindScore(Team team)
    {
        if (team == null)
        {
            throw new ArgumentNullException(nameof(team));
        }

        var score = scores.FirstOrDefault(x => x.Team.Equals(team));
        if (score == default)
        {
            throw new ArgumentException("Team did not participate in this match.", nameof(team));
        }

        return score;
    }
}

public readonly record struct TeamScore
{
    public TeamScore(Team team, int goals)
    {
        Team = team ?? throw new ArgumentNullException(nameof(team));

        if (goals < 0)
        {
            throw new ArgumentOutOfRangeException(nameof(goals), "Goals cannot be negative.");
        }

        Goals = goals;
    }

    public Team Team { get; }

    public int Goals { get; }
}
