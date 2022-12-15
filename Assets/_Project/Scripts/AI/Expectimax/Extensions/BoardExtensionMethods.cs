using System;
using System.Collections.Generic;
using System.Linq;

namespace _Project.Scripts.AI.Expectimax.Extensions
{
    // These methods allow us to essentially treat a ulong as the board state and perform operations on it.
    public static class BoardExtensionMethods
    {
        private static readonly Direction[] Directions = Enum.GetValues(typeof(Direction)).Cast<Direction>().Where(direction => direction != Direction.Undefined).ToArray();

        public static ulong MakeMove(this ulong state, Direction direction)
        {
            var table = direction is Direction.Left or Direction.Up ? LookupTables.LookupLeft : LookupTables.LookupRight;
            var transpose = direction is Direction.Up or Direction.Down; // need to transpose as we haven't precomputed states for these moves. 
            if (transpose) state = state.Transpose();
            state = table[(int)(state & 0xFFFF)] | ((ulong)table[(int)((state >> 16) & 0xFFFF)] << 16) | ((ulong)table[(int)((state >> 32) & 0xFFFF)] << 32) | ((ulong)table[(int)((state >> 48) & 0xFFFF)] << 48);
            if (transpose) state = state.Transpose();
            return state;
        }

        public static bool IsGameOver(this ulong state)
        {
            // ReSharper disable once LoopCanBeConvertedToQuery
            // Destroys performance using LINQ
            foreach (var direction in Directions)
                if (state != state.MakeMove(direction))
                    return false;

            return true;
        }

        public static uint GetScore(this ulong state) => state.GetLookupSum(LookupTables.LookupScores);

        public static double GetLookupSum(this ulong state, double[] lookupTable)
        {
            double score = 0;
            for (var i = 0; i < 4; i++) score += lookupTable[state >> i * 16 & 0xFFFF];
            return score;
        }
        
        public static uint GetLookupSum(this ulong state, uint[] lookupTable)
        {
            uint score = 0;
            for (var i = 0; i < 4; i++) score += lookupTable[state >> i * 16 & 0xFFFF];
            return score;
        }
        
        public static void SetTiles(this ref ulong state, IEnumerable<byte> tiles)
        {
            int i = 0;
            foreach (var tile in tiles) state |= (ulong)tile << (i++ * 4);
        }

        // Based on: https://stackoverflow.com/questions/53269297/transposing-a-4x4-matrix-represented-as-a-ulong-valueas-fast-as-possible
        public static ulong Transpose(this ulong state)
        {
            // 1111111111111111111111111111111111111111111111111111111111111111 -> 1111000011110000000011110000111111110000111100000000111100001111
            // R1-1, R1-3, R2-2, R2-4, R3-1, R3-3, R4-2, R4-4 
            var state1 = state & 0xF0F00F0FF0F00F0FL;
            // 1111111111111111111111111111111111111111111111111111111111111111 -> 0000000000000000111100001111000000000000000000001111000011110000
            // R2-1, R2-3, R4-1, R4-3
            var state2 = state & 0x0000F0F00000F0F0L;
            // 1111111111111111111111111111111111111111111111111111111111111111 -> 0000111100001111000000000000000000001111000011110000000000000000
            // R1-2, R1-4, R3-2, R3-4
            var state3 = state & 0x0F0F00000F0F0000L;
            // OR Left Shift 12, OR Right Shift 12. Overwrite state.
            state = state1 | (state2 << 12) | (state3 >> 12);
            // 1111111111111111111111111111111111111111111111111111111111111111 -> 1111111100000000111111110000000000000000111111110000000011111111
            // R1-1, R1-2, R2-1, R2-2, R3-3, R3-4, R4-3, R4-4
            state1 = state & 0xFF00FF0000FF00FFL;
            // 1111111111111111111111111111111111111111111111111111111111111111 -> 0000000011111111000000001111111100000000000000000000000000000000
            // R1-3, R1-4, R2-3, R2-4
            state2 = state & 0x00FF00FF00000000L;
            // 1111111111111111111111111111111111111111111111111111111111111111 -> 0000000000000000000000000000000011111111000000001111111100000000
            // R3-1, R3-2, R4-1, R4-2
            state3 = state & 0x00000000FF00FF00L;

            // OR Left Shift 24, OR Right Shift 24. Return transposed state.
            return state1 | (state2 >> 24) | (state3 << 24);
        }
    }
}