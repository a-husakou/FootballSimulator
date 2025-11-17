namespace FootballSimulator.Domain.Simulation
{
    public interface IGroupSimulator
    {
        GroupSimulationResult Simulate(GroupDefinition group);
    }
}
