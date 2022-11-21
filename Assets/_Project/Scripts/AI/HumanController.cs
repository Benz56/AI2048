using UnityEngine;

namespace _Project.Scripts.AI
{
    public class HumanController : MonoBehaviour
    {
        public AIManager aiManager;
        public GameController gameController;
        
        private void Update()
        {
            if (aiManager.aiType != AIType.HumanPlayer) return;
            if (gameController.Animating()) return;
            Direction? direction = null;
            if (Input.GetKeyDown("up") || Input.GetKeyDown("w"))
            {
                direction = Direction.Up;
            }
            else if (Input.GetKeyDown("down") || Input.GetKeyDown("s"))
            {
                direction = Direction.Down;
            }
            else if (Input.GetKeyDown("left") || Input.GetKeyDown("a"))
            {
                direction = Direction.Left;
            }
            else if (Input.GetKeyDown("right") || Input.GetKeyDown("d"))
            {
                direction = Direction.Right;
            }

            if (direction == null) return;

            gameController.MakeMove(direction.Value);
        }
    }
}