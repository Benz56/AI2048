namespace _Project.Scripts
{
    public class Cube
    {
        public Cube(int value = 0)
        {
            Value = value;
        }

        public int Value { get; set; }
        public bool IsZero => Value == 0;
    }
}