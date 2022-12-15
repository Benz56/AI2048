namespace _Project.Scripts.AI.Expectimax.Extensions
{
    public static class ByteArrayExtensions
    {
        private static byte[] MoveRight(this byte[] row)
        {
            for (var i = 0; i < 4; i++)
            {
                for (var j = 0; j < 3; j++)
                {
                    if (row[j + 1] != 0) continue;
                    row[j + 1] = row[j];
                    row[j] = 0;
                }
            }

            return row;
        }

        private static byte[] MergeRight(this byte[] row)
        {
            for (var i = 2; i > -1; i--)
            {
                if (row[i] != row[i + 1] || row[i] == 0) continue;
                row[i + 1] = (byte)(row[i + 1] + 1);
                row[i] = 0;
            }

            return row;
        }
        
        public static byte[] DoMoves(this byte[] row)
        {
            row = row.MoveRight();
            row = row.MergeRight();
            row = row.MoveRight();
            return row;
        }
    }
}