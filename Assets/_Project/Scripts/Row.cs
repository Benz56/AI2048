using System.Collections.Generic;
using System.Linq;

namespace _Project.Scripts
{
    public class Row
    {
        private readonly List<Cube> cubes;
        private bool IsEmpty => cubes.All(cube => cube.IsZero);

        public Row(List<Cube> cubes)
        {
            this.cubes = cubes;
        }

        public void Merge()
        {
            if (IsEmpty) return;
            
            var mergedRow = new List<Cube>();

            var rowToMerge = cubes.Where(cube => !cube.IsZero).ToList();
            if (rowToMerge.Count > 1)
            {
                // Try Merge
                for (var j = 0; j < rowToMerge.Count; j++)
                {
                    if (j < rowToMerge.Count - 1 && rowToMerge[j].Value == rowToMerge[j + 1].Value)
                    {
                        mergedRow.Add(new Cube(rowToMerge[j].Value * 2));
                        j++;
                    }
                    else mergedRow.Add(rowToMerge[j]);
                }
            }
            else mergedRow.Add(rowToMerge[0]);

            UpdateRow(mergedRow);
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