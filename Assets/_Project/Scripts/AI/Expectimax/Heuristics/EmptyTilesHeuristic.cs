using System;
using System.Linq;

namespace _Project.Scripts.AI.Expectimax.Heuristics
{
    internal class EmptyTilesHeuristic : AbstractHeuristic
    {
        public EmptyTilesHeuristic(double multiplier) : base(multiplier)
        {
        }

        public override double GetScore(byte[] row)
        {
            return row.Sum(x => x == 0 ? 1 : 0) * Multiplier;
        }
    }
}