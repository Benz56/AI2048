using System;

namespace _Project.Scripts.AI.Expectimax.Heuristics
{
    internal class AdjacencyHeuristic : AbstractHeuristic
    {
        // Two variations as described in paper.
        // Same tile value is favored.
        // Different tile value is penalized scaled by the difference.
        private readonly bool sameScoreTest;

        public AdjacencyHeuristic(double multiplier, double exponent, bool sameScoreTest) : base(multiplier, exponent)
        {
            this.sameScoreTest = sameScoreTest;
        }

        public override double GetScore(byte[] row)
        {
            double score = 0;
            
            for (var i = 0; i < 3; i++)
            {
                score += sameScoreTest ? SameTileValueTest(row[i], row[i + 1]) : DifferentTileValueTest(row[i], row[i + 1]);
            }

            return score * Multiplier;
        }

        private double DifferentTileValueTest(byte r1, byte r2)
        {
            return r1 != 0 && r1 != r2 ? Math.Pow(Math.Abs(r2 - r1), Exponent) : 0;
        }

        private double SameTileValueTest(byte r1, byte r2)
        {
            return r1 != 0 && r1 == r2 ? Math.Pow(1, Exponent) : 0;
        }
    }
}