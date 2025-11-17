using FootballSimulator.Domain.Configuration;
using FootballSimulator.Domain.Matches;
using FootballSimulator.Domain.Teams;

namespace FootballSimulator.Domain.Simulation.StandingCalculation
{
    public class StandingAccumulator
    {
        public StandingAccumulator(Team team)
        {
            Team = team;
        }

        public Team Team { get; }

        public int Played { get; private set; }
        public int Wins { get; private set; }
        public int Draws { get; private set; }
        public int Losses { get; private set; }
        public int GoalsFor { get; private set; }
        public int GoalsAgainst { get; private set; }
        public int Points { get; private set; }

        public int GoalDifference => GoalsFor - GoalsAgainst;

        public void ApplyMatch(MatchResult result)
        {
            if (result == null)
            {
                throw new ArgumentNullException(nameof(result));
            }

            Played++;

            var goalsFor = result.Score.GetGoalsFor(Team);
            var goalsAgainst = result.Score.GetGoalsAgainst(Team);
            GoalsFor += goalsFor;
            GoalsAgainst += goalsAgainst;

            if (goalsFor > goalsAgainst)
            {
                Wins++;
                Points += DomainConstants.Matches.PointsForWin;
            }
            else if (goalsFor == goalsAgainst)
            {
                Draws++;
                Points += DomainConstants.Matches.PointsForDraw;
            }
            else
            {
                Losses++;
            }
        }

        public GroupStanding ToStanding(int position) =>
            new(
                Team,
                Played,
                Wins,
                Draws,
                Losses,
                GoalsFor,
                GoalsAgainst,
                Points,
                position);
    }
}
