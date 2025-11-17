using FootballSimulator.Application;
using FootballSimulator.Domain.Matches;
using FootballSimulator.Domain.Simulation;

var seed = (int)DateTime.UtcNow.Ticks;
var runner = new RandomGroupSimulationRunner(seed);
var result = runner.RunOnce();

RenderSummary(result);

static void RenderSummary(GroupSimulationResult result)
{
    Console.WriteLine("Football Simulator");
    Console.WriteLine("------------------");
    Console.WriteLine("Group: " + result.Group.Name);

    Console.WriteLine();
    Console.WriteLine("Standings");
    Console.WriteLine(string.Format(
        "{0,3} {1,-23} {2,2} {3,2} {4,2} {5,2} {6,3} {7,3} {8,3} {9,4}",
        "Pos",
        "Team",
        "P",
        "W",
        "D",
        "L",
        "GF",
        "GA",
        "GD",
        "Pts"));

    foreach (var standing in result.Standings)
    {
        Console.WriteLine(
            $"{standing.Position,3} {standing.Team.Name,-23} {standing.Played,2} {standing.Wins,2} {standing.Draws,2} {standing.Losses,2} " +
            $"{standing.GoalsFor,3} {standing.GoalsAgainst,3} {standing.GoalDifference,3} {standing.Points,4}");
    }

    Console.WriteLine();
    Console.WriteLine("Matches");
    var number = 1;
    foreach (var match in result.Matches)
    {
        Console.WriteLine($"{number++,2}. {DescribeMatch(match)}");
    }
}

static string DescribeMatch(MatchResult match)
{
    var homeGoals = match.Score.GetGoalsFor(match.Fixture.HomeTeam);
    var awayGoals = match.Score.GetGoalsFor(match.Fixture.AwayTeam);
    var scoreLine = $"{homeGoals} - {awayGoals}";

    return $"Round {match.Fixture.Round}: {match.Fixture.HomeTeam.Name} vs {match.Fixture.AwayTeam.Name} => {scoreLine}";
}
