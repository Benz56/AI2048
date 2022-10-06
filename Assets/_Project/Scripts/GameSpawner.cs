using System;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

namespace _Project.Scripts
{
    public class GameSpawner : MonoBehaviour
    {
        public GameBoard gameBoard;
        public GameObject cubePrefab;

        private readonly Color[] textColors =
        {
            new(4, 4, 4),
            new(242, 235, 226)
        };

        private readonly List<Color> cubeColors = new()
        {
            new Color(238 / 255f, 228 / 255f, 218 / 255f),
            new Color(238 / 255f, 228 / 255f, 218 / 255f),
            new Color(237 / 255f, 224 / 255f, 200 / 255f),
            new Color(242 / 255f, 177 / 255f, 121 / 255f),
            new Color(245 / 255f, 149 / 255f, 99 / 255f),
            new Color(246 / 255f, 124 / 255f, 95 / 255f),
            new Color(246 / 255f, 94 / 255f, 59 / 255f),
            new Color(237 / 255f, 207 / 255f, 114 / 255f),
            new Color(237 / 255f, 204 / 255f, 97 / 255f),
            new Color(237 / 255f, 200 / 255f, 80 / 255f),
            new Color(237 / 255f, 200 / 255f, 80 / 255f),
            new Color(237 / 255f, 194 / 255f, 46 / 255f),
            new Color(60 / 255f, 58 / 255f, 50 / 255f)
        };

        private readonly SmartGrid<GameObject> cubesSmartGrid = new(BoardState.BoardSize);
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
                    cubesSmartGrid[x, y] = Instantiate(cubePrefab, new Vector3(x - BoardState.BoardSize / 2.0f + 0.5f, invertY - 0.5f, -4), Quaternion.identity, boardHolder.transform);
                }
            }

            GameBoard.OnGameBoardChanged += MoveCubes;
            MoveCubes(null);
            gameOverCheckCoroutine = StartCoroutine(CheckGameOver());
        }

        private IEnumerator<WaitForSeconds> CheckGameOver()
        {
            var wait = new WaitForSeconds(0.2f);
            while (true)
            {
                if (gameBoard.BoardState.IsGameOver())
                {
                    scoreTextMeshPro.faceColor = Color.red;
                    StopCoroutine(gameOverCheckCoroutine);
                }

                yield return wait;
            }
            // ReSharper disable once IteratorNeverReturns
        }

        private void MoveCubes(Direction? direction)
        {
            if (direction == null)
            {
                cubesSmartGrid.ForEach(SetCube);
            } else
            {
                cubesSmartGrid.ForEach(SetCube);
                // cubesSmartGrid.GetVectorsFromDirection(direction.Value).ForEach(list =>
                // {
                //     // TODO Compare content from board state with physical cubes and create animation.
                // });
            }

            if (gameBoard.BoardState.Score > 0)
            {
                SetScoreText(gameBoard.BoardState.Score);
            }
        }

        private void SetCube(int x, int y, GameObject cube)
        {
            var state = gameBoard.BoardState[x, y];
            if (state.IsZero)
            {
                cube.SetActive(false);
                return;
            }

            cube.SetActive(true);
            var changed = state.Value != (int.TryParse(cube.GetComponentInChildren<TextMeshProUGUI>().text, out var i) ? i : 0);
            cube.GetComponentInChildren<TextMeshProUGUI>().text = state.Value.ToString();
            cube.GetComponentInChildren<TextMeshProUGUI>().color = state.Value <= 4 ? textColors[0] : textColors[1];
            var geometricSeqToIndex = (int)(Mathf.Log(state.Value, 2) - 1);
            geometricSeqToIndex = geometricSeqToIndex > cubeColors.Count - 1 ? cubeColors.Count - 1 : geometricSeqToIndex;
            cube.GetComponent<MeshRenderer>().material.color = cubeColors[geometricSeqToIndex];
            if (changed)
            {
                StartCoroutine(Animate(cube));
            }
        }

        private void SetScoreText(int score)
        {
            scoreTextMeshPro.text = $"Score\n{score}";
        }

        private IEnumerator<WaitForSeconds> Animate(GameObject physicalCube)
        {
            // increase size
            physicalCube.transform.localScale *= 1.02f;
            yield return new WaitForSeconds(0.1f);
            physicalCube.transform.localScale = new Vector3(0.9f, 0.9f, 0.9f);
        }
    }

    [Serializable]
    public struct ValueColorCombo
    {
        public int value;
        public Color color;
    }
}