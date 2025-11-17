namespace FootballSimulator.Domain.Matches;

public interface IMatchSimulator
{
    MatchResult Simulate(MatchFixture fixture, double homeRating, double awayRating);
}
