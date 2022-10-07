using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace _Project.Scripts
{
    public class GameVectorLogic
    {
        private readonly List<Cube> cubes;
        private bool IsEmpty => cubes.All(cube => cube.IsZero);

        public GameVectorLogic(List<Cube> cubes)
        {
            this.cubes = cubes;
        }

        public MergeResult Merge()
        {
            var mergeResult = new MergeResult(0);
            if (IsEmpty) return mergeResult;

            var mergedRow = new List<int>();

            var rowToMerge = cubes.Where(cube => !cube.IsZero).ToList();
            if (rowToMerge.Count > 1)
            {
                // Try Merge
                for (var j = 0; j < rowToMerge.Count; j++)
                {
                    if (j < rowToMerge.Count - 1 && rowToMerge[j].Value == rowToMerge[j + 1].Value)
                    {
                        mergeResult.score += rowToMerge[j].Value * 2;
                        mergedRow.Add(rowToMerge[j].Value * 2);
                        mergeResult.mergedCubes.Add(((rowToMerge[j].Position, rowToMerge[j + 1].Position), cubes[mergedRow.Count - 1].Position)); // Test

                        j++;
                    }
                    else AddMoveResult(j);
                }
            }
            else AddMoveResult(0);

            void AddMoveResult(int i)
            {
                mergedRow.Add(rowToMerge[i].Value);
                if (rowToMerge[i].Position != cubes[mergedRow.Count - 1].Position)
                {
                    mergeResult.movedCubes.Add((rowToMerge[i].Position, cubes[mergedRow.Count - 1].Position)); // Test
                }
            }

            UpdateRow(mergedRow);
            return mergeResult;
        }

        private void UpdateRow(IReadOnlyList<int> mergedRow)
        {
            for (var j = 0; j < BoardState.BoardSize; j++)
            {
                cubes[j].Value = j < mergedRow.Count ? mergedRow[j] : 0;
            }
        }
    }

    public class MergeResult
    {
        public int score;
        public List<((int x, int y) from, (int x, int y) to)> movedCubes = new();
        public List<(((int x, int y) i, (int x, int y) j) from, (int x, int y) to)> mergedCubes = new();

        public MergeResult(int score)
        {
            this.score = score;
        }

        public override string ToString()
        {
            return $"Score: {score}, Moved Cubes: {movedCubes.Count}, Merged Cubes: {mergedCubes.Count}";
        }
    }
}