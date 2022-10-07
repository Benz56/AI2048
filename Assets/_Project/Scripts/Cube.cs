using UnityEngine;

namespace _Project.Scripts
{
    public class Cube
    {
        public (int x, int y) Position { get; }
        public int Value { get; set; }
        public bool IsZero => Value == 0;

        public Cube(int value, int x, int y)
        {
            Value = value;
            Position = (x, y);
        }
    }
}