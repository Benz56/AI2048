using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using UnityEngine;

namespace _Project.Scripts.AI
{
    public abstract class AbstractAI : MonoBehaviour
    {
        protected AIManager AiManager { get; private set; }
        protected GameController GameController { get; set; }
        protected BoardState BoardState { get; private set; }
        protected List<RunResult> Runs { get; } = new();

        public struct RunResult
        {
            public DateTime StartTime { get; set; }
            public DateTime EndTime { get; set; }
            public double AvgMoveTime { get; set; }
            public long Score { get; set; }
            public int MaxTile { get; set; }
        }

        public abstract AIType GetAiType();

        private DateTime startTime;
        private int moves;

        protected virtual void Start()
        {
            AiManager = transform.GetComponent<AIManager>();
            GameController = GameObject.Find("Game").GetComponent<GameController>();
            BoardState = GameController.BoardState;
        }

        private void Update()
        {
            if (!IsRunning()) return;
            SealedUpdate();
        }

        protected bool IsRunning() => !AiManager.paused && IsSelectedAIType() && CanMakeMove();

        public bool MakeMove(Direction direction)
        {
            moves++;
            return GameController.MakeMove(direction);
        }

        public bool CanMakeMove()
        {
            return !GameController.Animating();
        }

        public bool IsSelectedAIType()
        {
            return GetAiType() == AiManager.selectedAIType;
        }

        public abstract void SealedUpdate();

        protected void AddRun(RunResult runResult)
        {
            Runs.Add(runResult);
            if (Runs.Count >= AiManager.terminateAfterRuns)
            {
                UnityEditor.EditorApplication.isPlaying = false;
            }
        }

        protected void Reset()
        {
            var lastRun = Runs.Last();
            lastRun.AvgMoveTime = (double)(lastRun.EndTime - lastRun.StartTime).Milliseconds / moves;
            Debug.Log($"Run {Runs.Count} took {(lastRun.EndTime - lastRun.StartTime).TotalSeconds} seconds. Score was {lastRun.Score} and max tile was {lastRun.MaxTile}");
            GameController.Reset();
        }

        private void OnApplicationQuit()
        {
            if (Runs.Count == 0) return;
            var averageScore = Runs.Average(result => result.Score);
            var maxScore = Runs.Max(result => result.Score);
            var minScore = Runs.Min(result => result.Score);
            var averageTimeInSeconds = Runs.Average(result => (result.EndTime - result.StartTime).TotalSeconds);
            var maxTimeInSeconds = Runs.Max(result => (result.EndTime - result.StartTime).TotalSeconds);
            var minTimeInSeconds = Runs.Min(result => (result.EndTime - result.StartTime).TotalSeconds);
            var averageMoveTime = Runs.Average(result => result.AvgMoveTime);
            var maxTile = Runs.Max(result => result.MaxTile);
            var noRuns = Runs.Count;
            Debug.Log($"Average Score: {averageScore}");
            Debug.Log($"Max Score: {maxScore}");
            Debug.Log($"Min Score: {minScore}");
            Debug.Log($"Average Time: {averageTimeInSeconds}");
            Debug.Log($"Max Time: {maxTimeInSeconds}");
            Debug.Log($"Min Time: {minTimeInSeconds}");
            Debug.Log($"Average Move Time: {averageMoveTime}");
            Debug.Log($"Max Tile: {maxTile}");
            Debug.Log($"Runs: {noRuns}");
            StoreData();
        }

        private void StoreData()
        {
            var file = CreateFile();
            const string header = "Score;Max Tile;Avg Move Time;Run Time\n";
            File.WriteAllText(file, header);
            var data = Runs.Select(run => $"{run.Score};{run.MaxTile};{run.AvgMoveTime};{(run.EndTime - run.StartTime).TotalSeconds}");
            File.AppendAllLines(file, data);
            // Some pre-calculations.
            var averageScore = Runs.Average(result => result.Score);
            var maxScore = Runs.Max(result => result.Score);
            var minScore = Runs.Min(result => result.Score);
            var averageTimeInSeconds = Runs.Average(result => (result.EndTime - result.StartTime).TotalSeconds);
            var maxTimeInSeconds = Runs.Max(result => (result.EndTime - result.StartTime).TotalSeconds);
            var minTimeInSeconds = Runs.Min(result => (result.EndTime - result.StartTime).TotalSeconds);
            var totalTimeInSeconds = Runs.Sum(result => (result.EndTime - result.StartTime).TotalSeconds);
            var averageMoveTime = Runs.Average(result => result.AvgMoveTime);
            var maxTile = Runs.Max(result => result.MaxTile);
            var noRuns = Runs.Count;
            var percentage2048Tile = (double)Runs.Count(result => result.MaxTile >= 2048) / Runs.Count * 100; // Probability (in percentage) of getting a winning the game.
            var preCalculations = $@"
Average Score: {averageScore}
Max Score: {maxScore}
Min Score: {minScore}
Average Time: {averageTimeInSeconds}
Max Time: {maxTimeInSeconds}
Min Time: {minTimeInSeconds}
Total Time: {totalTimeInSeconds}
Average Move Time: {averageMoveTime}
Max Tile: {maxTile}
Runs: {noRuns}
Percentage of 2048 Tile: {percentage2048Tile}%
";
            for (var i = 1; i < 8; i++) // check some of the larger tiles up till the max of that game.
            {
                var tileValue = 2048 * Math.Pow(2, i); // 4096, 8192, 16384, 32768, 65536, 131072, 262144
                var percentageTile = (double)Runs.Count(result => result.MaxTile >= tileValue) / Runs.Count * 100; // Probability (in percentage) of getting the larger tiles.
                if (percentageTile == 0) break;
                preCalculations += $"Percentage of {tileValue} Tile: {percentageTile}%\n";
            }

            File.AppendAllText(file, preCalculations);
        }

        private string CreateFile()
        {
            var path = $"{Application.dataPath}/Data/";
            Directory.CreateDirectory(path);
            var fileName = $"{GetAiType()}.csv";
            path += fileName;
            // append number if file already exists
            var i = 1;
            while (File.Exists(path))
            {
                path = $"{path.Replace($"_{i - 1}.csv", $".csv")}"; // remove the previous number
                path = $"{path.Replace(".csv", $"_{i}.csv")}";
                i++;
            }

            return path;
        }
    }
}