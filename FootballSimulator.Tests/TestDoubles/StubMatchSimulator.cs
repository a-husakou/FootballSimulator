using FootballSimulator.Domain.Matches;
namespace FootballSimulator.Tests.TestDoubles;

public sealed class StubMatchSimulator : IMatchSimulator
{
    private readonly IReadOnlyDictionary<(string Home, string Away), (int HomeGoals, int AwayGoals)> matches;

    public StubMatchSimulator(IDictionary<(string Home, string Away), (int HomeGoals, int AwayGoals)> matches)
    {
        if (matches == null)
        {
            throw new ArgumentNullException(nameof(matches));
        }

        this.matches = new Dictionary<(string, string), (int, int)>(matches);
    }

    public MatchResult Simulate(MatchFixture fixture, double homeRating, double awayRating)
    {
        if (!matches.TryGetValue((fixture.HomeTeam.Name, fixture.AwayTeam.Name), out var score))
        {
            throw new InvalidOperationException(
                $"No scripted score for fixture {fixture.HomeTeam.Name} vs {fixture.AwayTeam.Name}.");
        }

        var scoreLine = new ScoreLine(fixture, score.HomeGoals, score.AwayGoals);
        return new MatchResult(fixture, scoreLine);
    }
}

