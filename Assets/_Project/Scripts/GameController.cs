using UnityEngine;

namespace _Project.Scripts
{
    public class GameController : MonoBehaviour, IResetAble
    {
        public GameSpawner gameSpawner;
        
        public static int AnimationCount { get; set; }
        public readonly BoardState BoardState = new();

        public void Start()
        {
            BoardState.SetRandomCube();
            BoardState.SetRandomCube();
        }

        [ContextMenu("Reset")]
        public void Reset()
        {
            BoardState.Reset();
            gameSpawner.Reset();
            BoardState.SetRandomCube();
            BoardState.SetRandomCube();
        }

        public bool Animating()
        {
            return AnimationCount > 0;
        }
        
        public bool MakeMove(Direction direction)
        {
            return BoardState.Move(direction);
        }

        public void GameOver()
        {
            Debug.Log("Game Over");
            Debug.Log(BoardState);
        }
    }
}