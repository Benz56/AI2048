namespace _Project.Scripts.AI.Expectimax.Heuristics
{
    public abstract class AbstractHeuristic
    {
        protected readonly double Multiplier;
        protected readonly double Exponent;

        protected AbstractHeuristic(double multiplier = 1, double exponent = 1)
        {
            Multiplier = multiplier;
            Exponent = exponent;
        }
        
        public abstract double GetScore(byte[] row);
    }
}