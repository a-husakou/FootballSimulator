using FootballSimulator.Domain.Configuration;
using FootballSimulator.Domain.Teams;

namespace FootballSimulator.Domain.Matches;

public record MatchFixture
{
    public MatchFixture(int round, Team homeTeam, Team awayTeam)
    {
        if (round < DomainConstants.Matches.MinimumRoundNumber)
        {
            throw new ArgumentOutOfRangeException(
                nameof(round),
                $"Round must be at least {DomainConstants.Matches.MinimumRoundNumber}.");
        }

        if (homeTeam == null)
        {
            throw new ArgumentNullException(nameof(homeTeam));
        }

        if (awayTeam == null)
        {
            throw new ArgumentNullException(nameof(awayTeam));
        }

        if (homeTeam.Equals(awayTeam))
        {
            throw new ArgumentException("A team cannot play against itself.", nameof(awayTeam));
        }

        Round = round;
        HomeTeam = homeTeam;
        AwayTeam = awayTeam;
    }

    public int Round { get; }

    public Team HomeTeam { get; }

    public Team AwayTeam { get; }

    public override string ToString() => $"Round {Round}: {HomeTeam.Name} vs {AwayTeam.Name}";
}
