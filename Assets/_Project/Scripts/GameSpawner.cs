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
        public List<ValueColorCombo> colorList;
        
        private readonly Dictionary<int, Color> colors = new();
        private readonly GameObject[,] cubes = new GameObject[4, 4];
        private GameObject boardHolder;

        private void Awake()
        {
            foreach (var valueColorCombo in colorList)
            {
                colors[valueColorCombo.value] = valueColorCombo.color;
            }
            GameBoard.OnGameBoardChanged += MoveCubes;
        }

        private void Start()
        {
            boardHolder = new GameObject("GameBoard");
            for (var x = 0; x < BoardState.BoardSize; x++)
            {
                for (var y = 0; y < BoardState.BoardSize; y++)
                {
                    var invertY = BoardState.BoardSize - y - 1; // invert y axis to match board state.
                    cubes[x, y] = Instantiate(cubePrefab, new Vector3(x - BoardState.BoardSize / 2.0f + 0.5f, invertY - 0.5f, -4), Quaternion.identity, boardHolder.transform);
                }
            }
        }

        private void MoveCubes()
        {
            for (var x = 0; x < BoardState.BoardSize; x++)
            {
                for (var y = 0; y < BoardState.BoardSize; y++)
                {
                    var state = gameBoard.BoardState[x, y];
                    var physicalCube = cubes[x, y];
                    physicalCube.GetComponentInChildren<TextMeshProUGUI>().text = state.Value == 0 ? "" : state.Value.ToString();
                    physicalCube.GetComponent<MeshRenderer>().material.color = colors.TryGetValue(state.Value, out var color) ? color : colors[colors.Keys.Max()];
                }
            }
        }
    }

    [Serializable]
    public struct ValueColorCombo
    {
        public int value;
        public Color color;
    }
}