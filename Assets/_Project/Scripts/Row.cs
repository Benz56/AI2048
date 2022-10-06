using System.Collections.Generic;
using System.Linq;

namespace _Project.Scripts
{
    public class Row
    {
        public readonly List<Cube> cubes;
        private bool IsEmpty => cubes.All(cube => cube.IsZero);

        public Row(List<Cube> cubes)
        {
            this.cubes = cubes;
        }

        public int Merge()
        {
            var score = 0;
            if (IsEmpty) return score;

            var mergedRow = new List<Cube>();

            var rowToMerge = cubes.Where(cube => !cube.IsZero).ToList();
            if (rowToMerge.Count > 1)
            {
                // Try Merge
                for (var j = 0; j < rowToMerge.Count; j++)
                {
                    if (j < rowToMerge.Count - 1 && rowToMerge[j].Value == rowToMerge[j + 1].Value)
                    {
                        score += rowToMerge[j].Value * 2;
                        mergedRow.Add(new Cube(rowToMerge[j].Value * 2));
                        j++;
                    }
                    else mergedRow.Add(rowToMerge[j]);
                }
            }
            else mergedRow.Add(rowToMerge[0]);

            UpdateRow(mergedRow);
            return score;
        }

        private void UpdateRow(IReadOnlyList<Cube> mergedRow)
        {
            for (var j = 0; j < BoardState.BoardSize; j++)
            {
                cubes[j].Value = j < mergedRow.Count ? mergedRow[j].Value : 0;
            }
        }
    }
}