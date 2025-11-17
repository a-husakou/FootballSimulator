namespace FootballSimulator.Domain.Algorithms;

public class SystemRandomProvider : IRandomProvider
{
    private readonly Random random;

    public SystemRandomProvider(int? seed = null)
    {
        random = seed.HasValue ? new Random(seed.Value) : Random.Shared;
    }

    public double NextDouble()
    {
        return random.NextDouble();
    }
}
