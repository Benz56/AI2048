using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using Random = System.Random;

namespace _Project.Scripts
{
    public class GameBoard : MonoBehaviour
    {
        public const int BoardSize = 4;
        public static float FourProbability = 0.1f;

        private readonly BoardState boardState = new();

        // Start is called before the first frame update
        void Start()
        {
            boardState.SetRandomCube();
            boardState.SetRandomCube();
            Debug.Log(boardState);
        }

        // Update is called once per frame
        void Update()
        {
            if (Input.GetKeyDown("up") || Input.GetKeyDown("w"))
            {
                boardState.Move(Direction.Up);
            }
            else if (Input.GetKeyDown("down") || Input.GetKeyDown("s"))
            {
                boardState.Move(Direction.Down);
            }
            else if (Input.GetKeyDown("left") || Input.GetKeyDown("a"))
            {
                boardState.Move(Direction.Left);
            }
            else if (Input.GetKeyDown("right") || Input.GetKeyDown("d"))
            {
                boardState.Move(Direction.Right);
            }
        }
    }

    internal class BoardState
    {
        private Cube[,] board =
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

        public void Move(Direction direction)
        {
            var initialState = board.Cast<Cube>().Select(cube => cube.Value).ToArray();

            var start = direction is Direction.Right or Direction.Down ? GameBoard.BoardSize - 1 : 0;
            var end = direction is Direction.Right or Direction.Down ? -1 : GameBoard.BoardSize;
            var step = direction is Direction.Right or Direction.Down ? -1 : 1;

            for (var i = start; i != end; i += step)
            {
                // Logic
                var row = GetRow();

                var nonZeroRow = row.Where(cube => cube.Value != 0).ToList();

                if (nonZeroRow.Count > 0)
                {
                    var mergedRow = MergeRow(nonZeroRow);
                    UpdateRow(mergedRow);
                }

                // Utility functions
                List<Cube> GetRow()
                {
                    var cubes = new List<Cube>();
                    for (var j = start; j != end; j += step)
                    {
                        var x = direction is Direction.Left or Direction.Right ? j : i;
                        var y = direction is Direction.Up or Direction.Down ? j : i;
                        cubes.Add(board[x, y]);
                    }

                    return cubes;
                }

                List<Cube> MergeRow(IReadOnlyList<Cube> rowToMerge)
                {
                    var merged = new List<Cube>();

                    if (rowToMerge.Count > 1)
                    {
                        // Try Merge
                        for (var j = 0; j < rowToMerge.Count; j++)
                        {
                            if (j < rowToMerge.Count - 1 && rowToMerge[j].Value == rowToMerge[j + 1].Value)
                            {
                                merged.Add(new Cube(rowToMerge[j].Value * 2));
                                j++;
                            }
                            else merged.Add(rowToMerge[j]);
                        }
                    }
                    else merged.Add(rowToMerge[0]);

                    return merged;
                }

                void UpdateRow(IReadOnlyList<Cube> mergedRow)
                {
                    for (var j = 0; j < GameBoard.BoardSize; j++)
                    {
                        row[j].Value = j < mergedRow.Count ? mergedRow[j].Value : 0;
                    }
                }
            }

            var newState = board.Cast<Cube>().Select(cube => cube.Value).ToArray();
            if (!initialState.SequenceEqual(newState))
            {
                SetRandomCube();
            }

            Debug.Log(this);
        }

        public bool IsGameOver()
        {
            if (EmptySlots().Count != 0)
            {
                return false;
            }

            // TODO Clean up. This is a mess.
            for (var x = 0; x < GameBoard.BoardSize; x++)
            {
                for (var y = 0; y < GameBoard.BoardSize; y++)
                {
                    // Check if there are any adjacent cubes on the x axis with the same value
                    for (var i = -1; i < 2; i += 2)
                    {
                        if (x + i < 0 || x + i >= GameBoard.BoardSize)
                        {
                            continue;
                        }

                        if (board[x, y].Value == board[x + i, y].Value)
                        {
                            return false;
                        }
                    }

                    // Check if there are any adjacent cubes on the y axis with the same value
                    for (var i = -1; i < 2; i += 2)
                    {
                        if (y + i < 0 || y + i >= GameBoard.BoardSize)
                        {
                            continue;
                        }

                        if (board[x, y].Value == board[x, y + i].Value)
                        {
                            return false;
                        }
                    }
                }
            }

            return true;
        }

        public List<(int, int)> EmptySlots()
        {
            var emptySlots = new List<(int x, int y)>();
            for (var x = 0; x < GameBoard.BoardSize; x++)
            {
                for (var y = 0; y < GameBoard.BoardSize; y++)
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
            board[x, y].Value = GameBoard.FourProbability >= new Random().NextDouble() ? 4 : 2;
            // board[0, 0].Value = 2;
            // board[1, 0].Value = 2;
            // board[2, 0].Value = 2;
            // board[3, 0].Value = 2;
            return true;
        }

        public override string ToString()
        {
            var boardString = IsGameOver() + "\n";
            for (var y = 0; y < GameBoard.BoardSize; y++)
            {
                for (var x = 0; x < GameBoard.BoardSize; x++)
                {
                    boardString += board[x, y].Value + " ";
                }

                boardString += "\n";
            }

            return boardString;
        }
    }

    internal enum Direction
    {
        Up,
        Down,
        Left,
        Right
    }

    internal class Cube
    {
        public Cube(int value = 0)
        {
            Value = value;
        }

        public int Value { get; set; }
        public bool IsEmpty => Value == 0;
    }
}