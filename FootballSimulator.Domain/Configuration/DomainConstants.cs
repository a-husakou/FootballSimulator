namespace FootballSimulator.Domain.Configuration;

public static class DomainConstants
{
    public static class Teams
    {
        public static class Strength
        {
            public const double MinRating = 0d;
            public const double MaxRating = 100d;

            public const double BaseRatingWeight = 0.6d;
            public const double FactorRatingWeight = 0.4d;

            public const double NormalizedMinValue = 0d;
            public const double NormalizedMaxValue = 1d;

            public const double MinAdjustmentPercentage = -1d;
            public const double MaxAdjustmentPercentage = 1d;
        }
    }

    public static class Matches
    {
        public const int MinimumRoundNumber = 1;

        public const int PointsForWin = 3;
        public const int PointsForDraw = 1;
        public const int PointsForLoss = 0;
    }

    public static class Groups
    {
        public const int StartingPosition = 1;
        public const int AdvancingPositions = 2;
    }
}
