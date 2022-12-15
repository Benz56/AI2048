namespace _Project.Scripts.AI.Expectimax.Heuristics
{
    // Tweak heuristic values here.
    public static class HeuristicConstants
    {
        public const float EmptyTilesMultiplier = 350;
        public const float MonotonyMultiplier = -3.5f;
        public const float MonotonyExponent = 3.8f;
        public const float EdgeMaxFlatMultiplier = 120;
        public const float EdgeMaxFlatExponent = 1;
        public const float EdgeMaxMultiplier = 2.1f;
        public const float EdgeMaxExponent = 1.8f;
        public const float AdjacencyPenaltyMultiplier = -0.6f;
        public const float AdjacencyPenaltyExponent = 1f;
        public const float AdjacencyBonusMultiplier = 650;
        public const float AdjacencyBonusExponent = 1f;
        public const int FlatRowBonus = 100_000;
    }
}