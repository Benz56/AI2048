using System.Collections.Generic;
using UnityEngine;

namespace _Project.Scripts
{
    public class SmartGrid<T> where T : new()
    {
        private readonly int size;
        private readonly T[,] grid;

        public SmartGrid(int size, bool fill)
        {
            this.size = size;
            grid = new T[size, size];
            if (!fill) return;
            for (var i = 0; i < size; i++)
            {
                for (var j = 0; j < size; j++)
                {
                    grid[i, j] = new T();
                }
            }
        }

        public T this[int x, int y]
        {
            get => grid[x, y];
            set => grid[x, y] = value;
        }

        public List<T> GetVectorInGrid(int i, Direction direction)
        {
            var entries = new List<T>();
            for (var j = 0; j < size; j++)
            {
                var x = direction is Direction.Left or Direction.Right ? j : i;
                var y = direction is Direction.Up or Direction.Down ? j : i;
                entries.Add(grid[x, y]);
            }
            
            if (direction is Direction.Right or Direction.Down)
                entries.Reverse();
            
            return entries;
        }
    }
}