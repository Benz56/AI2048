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
            BoardState.Reset();
        }

        [ContextMenu("Reset")]
        public void Reset()
        {
            BoardState.Reset();
            gameSpawner.Reset();
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
        }
    }
}