using System;

namespace _Project.Scripts.AI.Expectimax.Heuristics
{
    internal class MonotonyHeuristic : AbstractHeuristic
    {
        public MonotonyHeuristic(double multiplier = 1, double exponent = 1) : base(multiplier, exponent)
        {
        }

        public override double GetScore(byte[] row)
        {
            double monL = 0, monR = 0;
            
            for (var i = 1; i < 4; ++i)
            {
                if (row[i - 1] > row[i])
                    monL += Math.Pow(row[i - 1], Exponent) - Math.Pow(row[i], Exponent);
                else monR += Math.Pow(row[i], Exponent) - Math.Pow(row[i - 1], Exponent);
            }

            return Math.Min(monL, monR) * Multiplier;
        }
    }
}