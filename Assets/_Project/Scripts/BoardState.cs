using System;
using System.Collections.Generic;
using System.Linq;
using Random = System.Random;

namespace _Project.Scripts
{
    public class BoardState : IResetAble
    {
        public delegate void CubeSpawned(int x, int y, Cube cube);
        public static event CubeSpawned OnCubeSpawned;

        public delegate void MergeEvent(MergeResult result);
        public static event MergeEvent OnMerge;

        public delegate void ValidMoveEvent();
        public static event ValidMoveEvent OnValidMove;
        
        public const int BoardSize = 4;

        private const float FourProbability = 0.1f;
        public readonly SmartGrid<Cube> BoardSmartGrid = new(BoardSize);

        public int Score { get; set; }

        public BoardState()
        {
            Reset();
        }

        public Cube this[int x, int y]
        {
            get => BoardSmartGrid[x, y];
            set => BoardSmartGrid[x, y] = value;
        }

        public void Reset()
        {
            Score = 0;
            for (var i = 0; i < BoardSize; i++)
            {
                for (var j = 0; j < BoardSize; j++)
                {
                    BoardSmartGrid[i, j] = new Cube(0, i, j);
                }
            }
            SetRandomCube();
            SetRandomCube();
        }

        public bool Move(Direction direction)
        {
            if (direction == Direction.Undefined) throw new InvalidOperationException("Direction is undefined");
            var initialState = BoardSmartGrid.AsList().Select(cube => cube.Value).ToArray();

            BoardSmartGrid.GetVectorsFromDirection(direction).ForEach(vector =>
            {
                var result = new GameVectorLogic(vector).Merge();
                OnMerge?.Invoke(result);
                Score += result.score;
            });

            var newState = BoardSmartGrid.AsList().Select(cube => cube.Value).ToArray();
            if (initialState.SequenceEqual(newState))
            {
                return false;
            }
            
            SetRandomCube();
            OnValidMove?.Invoke();
            return true;
        }

        public bool IsGameOver()
        {
            if (EmptySlots().Count != 0)
            {
                return false;
            }

            // Check if any of the rows/columns can be merged. Does this by checking from two perpendicular sides and looking ahead if there is a merge possible.
            foreach (var vector in new List<Direction> { Direction.Left, Direction.Up }.SelectMany(direction => BoardSmartGrid.GetVectorsFromDirection(direction)))
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
                    if (BoardSmartGrid[x, y].Value == 0)
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
            BoardSmartGrid[x, y].Value = FourProbability >= new Random().NextDouble() ? 4 : 2;
            OnCubeSpawned?.Invoke(x, y, BoardSmartGrid[x, y]);
            return true;
        }

        public override string ToString()
        {
            var maxDigitLength = BoardSmartGrid.AsList().Select(cube => cube.Value).Max().ToString().Length;
            var boardString = IsGameOver() + "\n";
            for (var y = 0; y < BoardSize; y++)
            {
                for (var x = 0; x < BoardSize; x++)
                {
                    boardString += BoardSmartGrid[x, y].Value.ToString().PadRight(maxDigitLength, '_') + " ";
                }

                boardString += "\n";
            }

            return boardString;
        }

        public int GetMaxTile()
        {
            return BoardSmartGrid.AsList().Select(cube => cube.Value).Max();
        }
    }
}