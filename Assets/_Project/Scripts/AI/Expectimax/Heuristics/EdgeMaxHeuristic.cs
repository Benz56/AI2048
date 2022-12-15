using System;

namespace _Project.Scripts.AI.Expectimax.Heuristics
{
    internal class EdgeMaxHeuristic : AbstractHeuristic
    {
        private readonly bool scaleByValue;

        public EdgeMaxHeuristic(double multiplier, double exponent, bool scaleByValue) : base(multiplier, exponent)
        {
            this.scaleByValue = scaleByValue;
        }

        public override double GetScore(byte[] row)
        {
            double score = 0;

            if (row[0] != 0 && row[0] >= row[1] && row[0] >= row[2] && row[0] >= row[3])
            {
                score += scaleByValue ? Math.Pow(row[0], Exponent) : Math.Pow(1, Exponent);
            }

            if (row[3] != 0 && row[3] >= row[1] && row[3] >= row[2] && row[3] >= row[0])
            {
                score += scaleByValue ? Math.Pow(row[3], Exponent) : Math.Pow(1, Exponent);
            }

            return score * Multiplier;
        }
    }
}