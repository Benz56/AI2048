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

        private readonly GameObject[,] cubes = new GameObject[BoardState.BoardSize, BoardState.BoardSize];
        private GameObject boardHolder;
        private TextMeshPro scoreTextMeshPro;

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
                    cubes[x, y] = Instantiate(cubePrefab, new Vector3(x - BoardState.BoardSize / 2.0f + 0.5f, invertY - 0.5f, -4), Quaternion.identity, boardHolder.transform);
                }
            }

            GameBoard.OnGameBoardChanged += MoveCubes;
            MoveCubes();
        }

        private void SetScoreText(int score)
        {
            scoreTextMeshPro.text = $"Score\n{score}";
        }

        private void MoveCubes()
        {
            for (var x = 0; x < BoardState.BoardSize; x++)
            {
                for (var y = 0; y < BoardState.BoardSize; y++)
                {
                    var state = gameBoard.BoardState[x, y];
                    var physicalCube = cubes[x, y];
                    if (state.IsZero)
                    {
                        physicalCube.SetActive(false);
                        continue;
                    }

                    physicalCube.SetActive(true);
                    var changed = state.Value != (int.TryParse(physicalCube.GetComponentInChildren<TextMeshProUGUI>().text, out var i) ? i : 0);
                    physicalCube.GetComponentInChildren<TextMeshProUGUI>().text = state.Value.ToString();
                    physicalCube.GetComponentInChildren<TextMeshProUGUI>().color = state.Value <= 4 ? textColors[0] : textColors[1];
                    var geometricSeqToIndex = (int)(Mathf.Log(state.Value, 2) - 1);
                    geometricSeqToIndex = geometricSeqToIndex > cubeColors.Count - 1 ? cubeColors.Count - 1 : geometricSeqToIndex;
                    physicalCube.GetComponent<MeshRenderer>().material.color = cubeColors[geometricSeqToIndex];
                    if (changed)
                    {
                        StartCoroutine(Animate(physicalCube));
                    }
                }
            }

            if (gameBoard.BoardState.Score > 0)
            {
                SetScoreText(gameBoard.BoardState.Score);
            }
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