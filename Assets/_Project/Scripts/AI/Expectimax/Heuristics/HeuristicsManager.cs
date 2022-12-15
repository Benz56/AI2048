using System.Linq;
using _Project.Scripts.AI.Expectimax.Extensions;
using static _Project.Scripts.AI.Expectimax.Heuristics.HeuristicConstants;

namespace _Project.Scripts.AI.Expectimax.Heuristics
{
    internal static class HeuristicsManager
    {
        private const int Combinations16Bit = 65536;
        private static readonly AbstractHeuristic[] Heuristics =
        {
            new EmptyTilesHeuristic(EmptyTilesMultiplier), // bonus for empty tiles (the more the better)
            new MonotonyHeuristic(MonotonyMultiplier, MonotonyExponent), // penalty for monotony
            new EdgeMaxHeuristic(EdgeMaxFlatMultiplier, EdgeMaxFlatExponent, false), // bonus for max tiles along edges
            new EdgeMaxHeuristic(EdgeMaxMultiplier, EdgeMaxExponent, true), // exponential bonus for max tiles along edges
            new AdjacencyHeuristic(AdjacencyPenaltyMultiplier, AdjacencyPenaltyExponent,
                false), // penalty for adjacent tiles of different score. The penalty is scaled by absolute difference as we do want mergeable tiles to be close to each other.
            new AdjacencyHeuristic(AdjacencyBonusMultiplier, AdjacencyBonusExponent, true) // bonus for adjacent tiles of same score 
        };
        private static readonly double[] LookupTable = new double[Combinations16Bit]; // Precomputed values for all row combinations.

        static HeuristicsManager()
        {
            for (var i = 0; i < Combinations16Bit - 1; i++)
            {
                var row = GetRow(i);
                LookupTable[i] = Heuristics.Sum(heuristic => heuristic.GetScore(row)) + FlatRowBonus;
            }
        }

        private static byte[] GetRow(int i)
        {
            var row = new byte[4];
            for (byte k = 0; k < 4; k++) row[k] = (byte)((i >> k * 4) & 15);
            return row;
        }

        private static double GetHeuristicScore(this ulong state)
        {
            double score = 0;
            score += state.GetLookupSum(LookupTable);
            state = state.Transpose();
            score += state.GetLookupSum(LookupTable);
            return score;
        }

        public static double GetStateHeuristic(this ulong state, bool isGameOver = false)
        {
            return isGameOver || state.IsGameOver() ? 0 : state.GetHeuristicScore();
        }
    }
}