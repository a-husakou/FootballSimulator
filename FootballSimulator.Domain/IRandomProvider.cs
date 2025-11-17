namespace FootballSimulator.Domain;

public interface IRandomProvider
{
    double NextDouble();

    double NextDouble(double minValue, double maxValue)
    {
        if (minValue > maxValue)
        {
            throw new ArgumentOutOfRangeException(nameof(minValue), "Minimum must be smaller than maximum.");
        }

        return minValue + (maxValue - minValue) * NextDouble();
    }

    int Next(int minValueInclusive, int maxValueExclusive)
    {
        if (maxValueExclusive <= minValueInclusive)
        {
            throw new ArgumentOutOfRangeException(nameof(maxValueExclusive), "Maximum must be larger than minimum.");
        }

        var span = maxValueExclusive - minValueInclusive;
        return minValueInclusive + (int)Math.Floor(NextDouble() * span);
    }
}
