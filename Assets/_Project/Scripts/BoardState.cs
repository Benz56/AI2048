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
        private readonly SmartGrid<Cube> smartGrid = new(BoardSize, true);

        public int Score;

        public Cube this[int x, int y]
        {
            get => smartGrid[x, y];
            set => smartGrid[x, y] = value;
        }

        public bool Move(Direction direction)
        {
            var initialState = smartGrid.AsList().Select(cube => cube.Value).ToArray();

            smartGrid.GetVectorsFromDirection(direction).ForEach(vector => Score += new GameVectorLogic(vector).Merge());

            var newState = smartGrid.AsList().Select(cube => cube.Value).ToArray();
            if (initialState.SequenceEqual(newState))
            {
                return false;
            }

            SetRandomCube();
            return true;
        }

        public bool IsGameOver()
        {
            if (EmptySlots().Count != 0)
            {
                return false;
            }

            // Check if any of the rows/columns can be merged. Does this by checking from two perpendicular sides and looking ahead if there is a merge possible.
            foreach (var vector in new List<Direction> { Direction.Left, Direction.Up }.SelectMany(direction => smartGrid.GetVectorsFromDirection(direction)))
            {
                for (var i = 0; i < vector.Count -1; i++)
                {
                    if (vector[i].Value == vector[i + 1].Value)
                    {
                        return false;
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
                    if (smartGrid[x, y].Value == 0)
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
            smartGrid[x, y].Value = FourProbability >= new Random().NextDouble() ? 4 : 2;
            return true;
        }

        public override string ToString()
        {
            var maxDigitLength = smartGrid.AsList().Select(cube => cube.Value).Max().ToString().Length;
            var boardString = IsGameOver() + "\n";
            for (var y = 0; y < BoardSize; y++)
            {
                for (var x = 0; x < BoardSize; x++)
                {
                    boardString += smartGrid[x, y].Value.ToString().PadRight(maxDigitLength, '_') + " ";
                }

                boardString += "\n";
            }

            return boardString;
        }
    }
}