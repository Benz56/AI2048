using System;
using Unity.VisualScripting;
using UnityEngine;

namespace _Project.Scripts
{
    public class GameBoard : MonoBehaviour
    {
        public delegate void GameBoardChanged(Direction? direction);

        public static event GameBoardChanged OnGameBoardChanged;

        public readonly BoardState BoardState = new();

        // Start is called before the first frame update
        void Start()
        {
            BoardState.SetRandomCube();
            BoardState.SetRandomCube();
            OnGameBoardChanged?.Invoke(null);
        }

        // Update is called once per frame
        private void Update()
        {
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

            if (BoardState.Move(direction.Value))
            {
                OnGameBoardChanged?.Invoke(direction.Value);
            }
        }
    }
}