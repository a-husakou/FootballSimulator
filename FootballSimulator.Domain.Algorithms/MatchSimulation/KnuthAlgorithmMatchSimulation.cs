using FootballSimulator.Domain.Matches;

namespace FootballSimulator.Domain.Algorithms.MatchSimulation
{
    public class KnuthAlgorithmMatchSimulation : IMatchSimulator
    {
        private const double MinimumCombinedRating = 1d;
        private const double MinimumStrengthRatio = 0.05d;
        private const double MaximumStrengthRatio = 0.95d;
        private const double BalancedRatio = 0.5d;

        private readonly IRandomProvider random;
        private readonly MatchSimulationSettings settings;

        public KnuthAlgorithmMatchSimulation(IRandomProvider random, MatchSimulationSettings? settings = null)
        {
            this.random = random ?? throw new ArgumentNullException(nameof(random));
            this.settings = settings ?? MatchSimulationSettings.Default;
        }

        public MatchResult Simulate(MatchFixture fixture, double homeRating, double awayRating)
        {
            if (fixture == null)
            {
                throw new ArgumentNullException(nameof(fixture));
            }

            var homeLambda = CalculateLambda(homeRating, awayRating);
            var awayLambda = CalculateLambda(awayRating, homeRating);

            var homeGoals = SamplePoisson(homeLambda);
            var awayGoals = SamplePoisson(awayLambda);

            var score = new ScoreLine(fixture, homeGoals, awayGoals);
            return new MatchResult(fixture, score);
        }

        private double CalculateLambda(double attackRating, double opponentRating)
        {
            var total = Math.Max(attackRating + opponentRating, MinimumCombinedRating);
            var ratio = Math.Clamp(attackRating / total, MinimumStrengthRatio, MaximumStrengthRatio);

            var lambda = settings.BaseExpectedGoals + settings.StrengthImpact * (ratio - BalancedRatio);
            return Math.Clamp(lambda, settings.MinimumLambda, settings.MaximumLambda);
        }

        private int SamplePoisson(double lambda)
        {
            var l = Math.Exp(-lambda);
            var k = 0;
            var p = 1.0;

            do
            {
                k++;
                p *= random.NextDouble();
            }
            while (p > l);

            return k - 1;
        }
    }
}
