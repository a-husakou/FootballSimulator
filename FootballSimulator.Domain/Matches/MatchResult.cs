using FootballSimulator.Domain.Teams;

namespace FootballSimulator.Domain.Matches;

public class MatchResult
{
    public MatchResult(MatchFixture fixture, ScoreLine score)
    {
        Fixture = fixture ?? throw new ArgumentNullException(nameof(fixture));
        Score = score ?? throw new ArgumentNullException(nameof(score));

        if (!Score.InvolvesTeam(Fixture.HomeTeam) || !Score.InvolvesTeam(Fixture.AwayTeam))
        {
            throw new ArgumentException("Score line teams do not match the fixture.", nameof(score));
        }
    }

    public MatchFixture Fixture { get; }

    public ScoreLine Score { get; }

    public bool IsDraw => Score.IsDraw;

    public Team? Winner => Score.GetWinner();

    public Team? Loser => Score.GetLoser();

    public int GetGoalsFor(Team team)
    {
        if (team == null)
        {
            throw new ArgumentNullException(nameof(team));
        }

        return Score.GetGoalsFor(team);
    }

    public int GetGoalsAgainst(Team team) => Score.GetGoalsAgainst(team);

    public Team GetOpponent(Team team)
    {
        if (team == null)
        {
            throw new ArgumentNullException(nameof(team));
        }

        return Score.GetOpponent(team);
    }
}
