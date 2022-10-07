using System;
using Unity.VisualScripting;
using UnityEngine;

namespace _Project.Scripts
{
    public class GameBoard : MonoBehaviour
    {
        public static int AnimationCount { get; set; }

        public readonly BoardState BoardState = new();

        // Start is called before the first frame update
        public void Start()
        {
            BoardState.SetRandomCube();
            BoardState.SetRandomCube();
        }

        // Update is called once per frame
        private void Update()
        {
            if (AnimationCount > 0) return;
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

            BoardState.Move(direction.Value);
        }
    }
}