using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using Random = System.Random;

namespace _Project.Scripts
{
    public class BoardState
    {
        public const int BoardSize = 4;
        private const float FourProbability = 0.1f;
        
        private readonly Cube[,] board =
        {
            { new(), new(), new(), new() },
            { new(), new(), new(), new() },
            { new(), new(), new(), new() },
            { new(), new(), new(), new() }
        };

        public Cube this[int x, int y]
        {
            get => board[x, y];
            set => board[x, y] = value;
        }

        public bool Move(Direction direction)
        {
            var initialState = board.Cast<Cube>().Select(cube => cube.Value).ToArray();

            var start = direction is Direction.Right or Direction.Down ? BoardSize - 1 : 0;
            var end = direction is Direction.Right or Direction.Down ? -1 : BoardSize;
            var step = direction is Direction.Right or Direction.Down ? -1 : 1;

            for (var i = start; i != end; i += step)
            {
                var row = GetRow();
                row.Merge();

                Row GetRow()
                {
                    var cubes = new List<Cube>();
                    for (var j = start; j != end; j += step)
                    {
                        var x = direction is Direction.Left or Direction.Right ? j : i;
                        var y = direction is Direction.Up or Direction.Down ? j : i;
                        cubes.Add(board[x, y]);
                    }

                    return new Row(cubes);
                }
            }

            var newState = board.Cast<Cube>().Select(cube => cube.Value).ToArray();
            if (!initialState.SequenceEqual(newState))
            {
                SetRandomCube();
                Debug.Log(this);
                return true;
            }

            Debug.Log(this);
            return false;
        }

        public bool IsGameOver()
        {
            if (EmptySlots().Count != 0)
            {
                return false;
            }

            // Quite a few redundant checks here, but it's is simpler to implement.
            for (var x = 0; x < BoardSize; x++)
            {
                for (var y = 0; y < BoardSize; y++)
                {
                    for (var i = -1; i < 2; i += 2)
                    {
                        // Check x for merge opportunities
                        if (x + i >= 0 && x + i < BoardSize && board[x, y].Value == board[x + i, y].Value)
                        {
                            return false;
                        }

                        // Check y for merge opportunities
                        if (y + i >= 0 && y + i < BoardSize && board[x, y].Value == board[x, y + i].Value)
                        {
                            return false;
                        }
                    }
                }
            }

            return true;
        }

        private List<(int, int)> EmptySlots()
        {
            var emptySlots = new List<(int x, int y)>();
            for (var x = 0; x < BoardSize; x++)
            {
                for (var y = 0; y < BoardSize; y++)
                {
                    if (board[x, y].Value == 0)
                    {
                        emptySlots.Add((x, y));
                    }
                }
            }

            return emptySlots;
        }

        public bool SetRandomCube()
        {
            var emptySlots = EmptySlots();
            if (emptySlots.Count == 0)
            {
                return false;
            }

            var (x, y) = emptySlots[new Random().Next(emptySlots.Count)];
            board[x, y].Value = FourProbability >= new Random().NextDouble() ? 4 : 2;
            return true;
        }

        public override string ToString()
        {
            var maxDigitLength = board.Cast<Cube>().Select(cube => cube.Value).Max().ToString().Length;
            var boardString = IsGameOver() + "\n";
            for (var y = 0; y < BoardSize; y++)
            {
                for (var x = 0; x < BoardSize; x++)
                {
                    boardString += board[x, y].Value.ToString().PadRight(maxDigitLength, '_') + " ";
                }

                boardString += "\n";
            }

            return boardString;
        }
    }
}