using UnityEngine;

namespace _Project.Scripts
{
    public class GameController : MonoBehaviour
    {
        public static int AnimationCount { get; set; }
        public readonly BoardState BoardState = new();

        public void Start()
        {
            BoardState.SetRandomCube();
            BoardState.SetRandomCube();
        }

        public bool Animating()
        {
            return AnimationCount > 0;
        }
        
        public void MakeMove(Direction direction)
        {
            BoardState.Move(direction);
        }
    }
}