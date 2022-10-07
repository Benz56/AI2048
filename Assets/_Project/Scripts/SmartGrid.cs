using System;
using System.Collections.Generic;
using System.Linq;

namespace _Project.Scripts
{
    public class SmartGrid<T> where T : new()
    {
        private readonly int size;
        private readonly T[,] grid;

        public SmartGrid(int size, bool fill = false)
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

        public List<List<T>> GetVectorsFromDirection(Direction direction)
        {
            var vectors = new List<List<T>>();
            for (var i = 0; i < size; i++)
            {
                vectors.Add(GetVectorInGrid(i, direction));
            }

            return vectors;
        }

        public List<T> AsList()
        {
            return grid.Cast<T>().ToList();
        }

        public void ForEach(Action<int, int, T> action)
        {
            for (var x = 0; x < size; x++)
            {
                for (var y = 0; y < size; y++)
                {
                    action(x, y, grid[x, y]);
                }
            }
        }
    }
}