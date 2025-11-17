using FootballSimulator.Domain.Configuration;
using FootballSimulator.Domain.Matches;
using FootballSimulator.Domain.Simulation.StandingCalculation;
using FootballSimulator.Domain.Teams;

namespace FootballSimulator.Domain.Simulation;

public class DefaultGroupSimulator : IGroupSimulator
{
    private readonly IMatchSimulator matchSimulator;
    private readonly IGroupScheduler groupScheduler;
    private readonly IMatchModifierEventProvider eventProvider;

    public DefaultGroupSimulator(IMatchSimulator matchSimulator, IGroupScheduler groupScheduler, IMatchModifierEventProvider eventProvider)
    {
        this.matchSimulator = matchSimulator ?? throw new ArgumentNullException(nameof(matchSimulator));
        this.groupScheduler = groupScheduler ?? throw new ArgumentNullException(nameof(groupScheduler));
        this.eventProvider = eventProvider ?? throw new ArgumentNullException(nameof(eventProvider));
    }

    public GroupSimulationResult Simulate(
        GroupDefinition group)
    {
        if (group == null)
        {
            throw new ArgumentNullException(nameof(group));
        }

        var fixtures = groupScheduler.GenerateFixtures(group);
        var matches = new List<MatchResult>(fixtures.Count);
        var accumulators = group.Teams.ToDictionary(team => team, team => new StandingAccumulator(team));

        foreach (var fixture in fixtures)
        {
            var events = eventProvider.GetEventsForMatch(fixture);

            var homeRating = ResolveRating(fixture.HomeTeam, events, includeHomeAdvantage: true);
            var awayRating = ResolveRating(fixture.AwayTeam, events, includeHomeAdvantage: false);

            var result = matchSimulator.Simulate(fixture, homeRating, awayRating);
            matches.Add(result);

            accumulators[fixture.HomeTeam].ApplyMatch(result);
            accumulators[fixture.AwayTeam].ApplyMatch(result);
        }

        var orderedAccumulators = accumulators.Values.ToList();
        orderedAccumulators.Sort(new StandingComparer(matches));

        var standings = new List<GroupStanding>(orderedAccumulators.Count);
        var position = DomainConstants.Groups.StartingPosition;
        foreach (var accumulator in orderedAccumulators)
        {
            standings.Add(accumulator.ToStanding(position++));
        }

        return new GroupSimulationResult(group, matches, standings);
    }

    private double ResolveRating(
        Team team,
        IReadOnlyCollection<StrengthModifierName>? events,
        bool includeHomeAdvantage)
    {
        IEnumerable<StrengthModifierName>? effectiveEvents = events is { Count: > 0 } ? events : null;

        if (includeHomeAdvantage)
        {
            if (effectiveEvents == null)
            {
                effectiveEvents = new[] { StrengthModifierName.HomeAdvantage };
            }
            else if (!events!.Contains(StrengthModifierName.HomeAdvantage))
            {
                effectiveEvents = effectiveEvents.Append(StrengthModifierName.HomeAdvantage);
            }
        }

        return effectiveEvents == null
            ? team.BaseStrength.Rating
            : team.CalculateRating(effectiveEvents);
    }
}
