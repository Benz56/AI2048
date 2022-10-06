using System;
using Unity.VisualScripting;
using UnityEngine;

namespace _Project.Scripts
{
    public class GameBoard : MonoBehaviour
    {
        public delegate void GameBoardChanged();
        public static event GameBoardChanged OnGameBoardChanged;

        public readonly BoardState BoardState = new();

        // Start is called before the first frame update
        void Start()
        {
            BoardState.SetRandomCube();
            BoardState.SetRandomCube();
            OnGameBoardChanged?.Invoke();
        }

        // Update is called once per frame
        private void Update()
        {
            var moved = false;
            if (Input.GetKeyDown("up") || Input.GetKeyDown("w"))
            {
                moved = BoardState.Move(Direction.Up);
            }
            else if (Input.GetKeyDown("down") || Input.GetKeyDown("s"))
            {
                moved = BoardState.Move(Direction.Down);
            }
            else if (Input.GetKeyDown("left") || Input.GetKeyDown("a"))
            {
                moved = BoardState.Move(Direction.Left);
            }
            else if (Input.GetKeyDown("right") || Input.GetKeyDown("d"))
            {
                moved = BoardState.Move(Direction.Right);
            }

            if (moved)
            {
                // trigger move event
                OnGameBoardChanged?.Invoke();
            }
        }
    }
}