using FootballSimulator.Domain.Matches;
using FootballSimulator.Domain.Simulation;
using FootballSimulator.Domain.Teams;
using FootballSimulator.Domain;


namespace FootballSimulator.Application
{
    public class RandomMatchModifierEventProvider : IMatchModifierEventProvider
    {
        private static readonly IReadOnlyList<StrengthModifierName> ModifierPool =
            Enum.GetValues<StrengthModifierName>()
                .Where(modifier => modifier != StrengthModifierName.HomeAdvantage)
                .ToArray();

        private readonly IRandomProvider random;

        public RandomMatchModifierEventProvider(IRandomProvider random)
        {
            this.random = random ?? throw new ArgumentNullException(nameof(random));
        }

        public IReadOnlyCollection<StrengthModifierName>? GetEventsForMatch(MatchFixture fixture)
        {
            if (ModifierPool.Count == 0)
            {
                return null;
            }

            var picks = new List<StrengthModifierName>();
            foreach (var modifier in ModifierPool)
            {
                if (random.NextDouble() < 0.5)
                {
                    picks.Add(modifier);
                }
            }

            return picks.Count == 0 ? null : picks.AsReadOnly();
        }
    }
}
