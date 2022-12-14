using System.Collections.Generic;
using System.Linq;
using TMPro;
using UnityEngine;

namespace _Project.Scripts
{
    public class GameSpawner : MonoBehaviour, IResetAble
    {
        public GameController gameController;
        public GameObject cubePrefab;
        public bool animations;

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

            BoardState.OnCubeSpawned += CubeSpawned;
            BoardState.OnMerge += HandleMerge;
            BoardState.OnValidMove += OnValidMove;
            cubesSmartGrid.ForEach((x, y, prefab) => prefab.SetState(gameController.BoardState[x, y], animations));
        }

        public void Reset()
        {
            SetScoreText(0);
            scoreTextMeshPro.color = Color.white;
            cubesSmartGrid.ForEach((x, y, prefab) => prefab.SetState(gameController.BoardState[x, y], animations));
        }

        // TODO Visual glitch when not animating.
        private void HandleMerge(MergeResult result)
        {
            foreach (var (from, to) in result.movedCubes)
            {
                cubesSmartGrid[to.x, to.y].SetState(gameController.BoardState[to.x, to.y], animations);
                cubesSmartGrid[from.x, from.y].CreateMoveAnimation(cubesSmartGrid[to.x, to.y], true, animations);
            }

            foreach (var ((from1, from2), to) in result.mergedCubes)
            {
                cubesSmartGrid[to.x, to.y].SetState(gameController.BoardState[to.x, to.y], animations);
                if (from1 != to)
                {
                    cubesSmartGrid[from1.x, from1.y].CreateMoveAnimation(cubesSmartGrid[to.x, to.y], from2 != to, animations);
                }

                if (from2 != to)
                {
                    cubesSmartGrid[from2.x, from2.y].CreateMoveAnimation(cubesSmartGrid[to.x, to.y], from1 != to, animations);
                }
            }

            if (result.score > 0)
            {
                SetScoreText(gameController.BoardState.Score);
            }
        }

        private void OnValidMove()
        {
            if (!gameController.BoardState.IsGameOver()) return;
            gameController.GameOver();
            scoreTextMeshPro.color = Color.red;
        }

        private void CubeSpawned(int x, int y, Cube cube)
        {
            cubesSmartGrid[x, y].SetState(cube, animations);
            cubesSmartGrid[x, y].Visible(true);
        }

        private void SetScoreText(int score)
        {
            scoreTextMeshPro.text = $"Score\n{score}";
        }

        public override string ToString()
        {
            var maxDigitLength = cubesSmartGrid.AsList().Select(cube => cube.value).Max().ToString().Length;
            var boardString = gameController.BoardState.IsGameOver() + "\n";
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