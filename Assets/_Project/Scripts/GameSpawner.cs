using System;
using System.Collections.Generic;
using System.Linq;
using TMPro;
using UnityEngine;

namespace _Project.Scripts
{
    public class GameSpawner : MonoBehaviour
    {
        public GameBoard gameBoard;
        public GameObject cubePrefab;

        private readonly SmartGrid<CubePrefab> cubesSmartGrid = new(BoardState.BoardSize);
        private GameObject boardHolder;
        private TextMeshPro scoreTextMeshPro;
        private Coroutine gameOverCheckCoroutine;

        private void Start()
        {
            boardHolder = new GameObject("GameBoard");
            var scoreText = new GameObject("ScoreText");
            scoreText.transform.SetParent(boardHolder.transform);
            scoreText.transform.localScale = new Vector3(0.1f, 0.1f, 0.1f);
            scoreTextMeshPro = scoreText.AddComponent<TextMeshPro>();
            scoreTextMeshPro.alignment = TextAlignmentOptions.Center;
            scoreTextMeshPro.fontStyle = FontStyles.Bold;
            scoreText.transform.position = new Vector3(0, BoardState.BoardSize / 2f + 1.3f, -5);
            SetScoreText(0);

            for (var x = 0; x < BoardState.BoardSize; x++)
            {
                for (var y = 0; y < BoardState.BoardSize; y++)
                {
                    var invertY = BoardState.BoardSize - y - 1; // invert y axis to match board state.
                    cubesSmartGrid[x, y] = Instantiate(cubePrefab, new Vector3(x - BoardState.BoardSize / 2.0f + 0.5f, invertY - 0.5f, -4), Quaternion.identity, boardHolder.transform).GetComponent<CubePrefab>();
                }
            }

            GameBoard.OnGameBoardChanged += MoveCubes;
            BoardState.OnCubeSpawned += CubeSpawned;
            MoveCubes(null);
            StartCoroutine(CheckGameOver());
        }

        private IEnumerator<WaitForSeconds> CheckGameOver()
        {
            var wait = new WaitForSeconds(0.2f);
            while (true)
            {
                if (gameBoard.BoardState.IsGameOver())
                {
                    scoreTextMeshPro.faceColor = Color.red;
                    yield break;
                }

                yield return wait;
            }
            // ReSharper disable once IteratorNeverReturns
        }

        private void MoveCubes(Direction? direction)
        {
            Debug.Log("?");
            if (direction == null)
            {
                cubesSmartGrid.ForEach((x, y, prefab) => prefab.SetState(gameBoard.BoardState[x, y]));
            } else
            {
                for (var i = 0; i < BoardState.BoardSize; i++)
                {
                    var physicalCubes = cubesSmartGrid.GetVectorInGrid(i, direction.Value);
                    var boardCubes = gameBoard.BoardState.boardSmartGrid.GetVectorInGrid(i, direction.Value);
                    var filled = physicalCubes[0].value != 0 ? 1 : 0;
                    for (var j = 1; j < BoardState.BoardSize; j++)
                    {
                        if (physicalCubes[j].value == 0 || physicalCubes[j].ignoreInAnimation)
                        {
                            physicalCubes[j].ignoreInAnimation = false; // Used to ignore new tiles.
                            continue;
                        }
                        physicalCubes[j].ignoreInAnimation = false; // Used to ignore new tiles.

                        if (j != filled)
                        {
                            Debug.Log("Animation of " + physicalCubes[j].value + " from " + j + " to " + filled);
                            physicalCubes[j].CreateMoveAnimation(physicalCubes[filled]);
                            physicalCubes[filled].SetState(boardCubes[filled]);
                        }

                        filled++;
                    }
                }
            }

            if (gameBoard.BoardState.Score > 0)
            {
                SetScoreText(gameBoard.BoardState.Score);
            }
        }

        private void CubeSpawned(int x, int y, Cube cube)
        {
            Debug.Log("2");
            cubesSmartGrid[x, y].SetState(cube);
            cubesSmartGrid[x, y].ignoreInAnimation = true;
            cubesSmartGrid[x, y].Visible(true);
        }

        private void SetScoreText(int score)
        {
            scoreTextMeshPro.text = $"Score\n{score}";
        }
        
        public override string ToString()
        {
            var maxDigitLength = cubesSmartGrid.AsList().Select(cube => cube.value).Max().ToString().Length;
            var boardString = gameBoard.BoardState.IsGameOver() + "\n";
            for (var y = 0; y < BoardState.BoardSize; y++)
            {
                for (var x = 0; x < BoardState.BoardSize; x++)
                {
                    boardString += cubesSmartGrid[x, y].value.ToString().PadRight(maxDigitLength, '_') + " ";
                }

                boardString += "\n";
            }

            return boardString;
        }
    }
}