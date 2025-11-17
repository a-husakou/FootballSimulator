using FootballSimulator.Domain.Configuration;
using FootballSimulator.Domain.Matches;
using FootballSimulator.Domain.Teams;

namespace FootballSimulator.Domain.Simulation.StandingCalculation
{
    public class StandingComparer : IComparer<StandingAccumulator>
    {
        private readonly IReadOnlyCollection<MatchResult> matches;

        public StandingComparer(IReadOnlyCollection<MatchResult> matches)
        {
            this.matches = matches;
        }

        public int Compare(StandingAccumulator? x, StandingAccumulator? y)
        {
            if (x == null || y == null)
            {
                return 0;
            }

            var compare = y.Points.CompareTo(x.Points);
            if (compare != 0)
            {
                return compare;
            }

            compare = y.GoalDifference.CompareTo(x.GoalDifference);
            if (compare != 0)
            {
                return compare;
            }

            compare = y.GoalsFor.CompareTo(x.GoalsFor);
            if (compare != 0)
            {
                return compare;
            }

            compare = x.GoalsAgainst.CompareTo(y.GoalsAgainst);
            if (compare != 0)
            {
                return compare;
            }

            return CompareHeadToHead(x.Team, y.Team);
        }

        private int CompareHeadToHead(Team teamA, Team teamB)
        {
            var relevantMatches = matches.Where(match =>
                    (match.Fixture.HomeTeam.Equals(teamA) && match.Fixture.AwayTeam.Equals(teamB)) ||
                    (match.Fixture.HomeTeam.Equals(teamB) && match.Fixture.AwayTeam.Equals(teamA)))
                .ToList();

            if (relevantMatches.Count == 0)
            {
                return 0;
            }

            var statsA = CalculateHeadToHeadStatistic(teamA, relevantMatches);
            var statsB = CalculateHeadToHeadStatistic(teamB, relevantMatches);

            var compare = statsB.Points.CompareTo(statsA.Points);
            if (compare != 0)
            {
                return compare;
            }

            compare = statsB.GoalDifference.CompareTo(statsA.GoalDifference);
            if (compare != 0)
            {
                return compare;
            }

            compare = statsB.GoalsFor.CompareTo(statsA.GoalsFor);
            return compare;
        }

        private static (int Points, int GoalDifference, int GoalsFor) CalculateHeadToHeadStatistic(Team team, IEnumerable<MatchResult> matches)
        {
            var points = 0;
            var goalsFor = 0;
            var goalsAgainst = 0;

            foreach (var match in matches)
            {
                points += CalculatePoints(match, team);
                goalsFor += match.GetGoalsFor(team);
                goalsAgainst += match.GetGoalsAgainst(team);
            }

            return (points, goalsFor - goalsAgainst, goalsFor);
        }

        private static int CalculatePoints(MatchResult match, Team team)
        {
            if (match.Score.IsDraw)
            {
                return DomainConstants.Matches.PointsForDraw;
            }

            if (match.Winner?.Equals(team) == true)
            {
                return DomainConstants.Matches.PointsForWin;
            }

            if (!match.Score.InvolvesTeam(team))
            {
                throw new ArgumentException("Team did not participate in this match.", nameof(team));
            }

            return DomainConstants.Matches.PointsForLoss;
        }
    }
}
