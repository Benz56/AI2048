using _Project.Scripts.AI.Expectimax.Extensions;

namespace _Project.Scripts.AI.Expectimax
{
    // Lookup tables for performance.
    public static class LookupTables
    {
        private const int Combinations16Bit = 65536; // 2^16
        public static readonly uint[] LookupRight = new uint[Combinations16Bit];
        public static readonly uint[] LookupLeft = new uint[Combinations16Bit];
        public static readonly uint[] LookupScores = new uint[Combinations16Bit];

        static LookupTables()
        {
            for (var i = 0; i < Combinations16Bit; i++)
            {
                RightLookup(i);
                LeftLookup(i);
            }
        }

        private static void RightLookup(int i)
        {
            var row = new byte[4];
            var score = 0;
            for (byte k = 0; k < 4; k++)
            {
                row[k] = (byte)((i >> k * 4) & 15);
                if (row[k] >= 2) score += (row[k] - 1) * (1 << row[k]);
            }

            row = row.DoMoves();
            LookupRight[i] = (uint)(row[0] + (row[1] << 4) + (row[2] << 8) + (row[3] << 12));
            LookupScores[i] = (uint)score;
        }

        private static void LeftLookup(int i)
        {
            var row = new byte[4];

            for (byte k = 0; k < 4; k++)
            {
                row[3 - k] = (byte)((i >> k * 4) & 15);
            }

            row = row.DoMoves();
            LookupLeft[i] = (uint)(row[3] + (row[2] << 4) + (row[1] << 8) + (row[0] << 12));
        }
    }
}