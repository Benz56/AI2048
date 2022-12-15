using System;
using System.Collections;
using System.Collections.Generic;
using _Project.Scripts.AI.Expectimax.Extensions;
using UnityEngine;

namespace _Project.Scripts.AI.Expectimax
{
    public class ExpectimaxAI : AbstractAI
    {
        [Range(1, 20)]
        public int searchDepth = 4;
        [Range(0, 0.1f)]
        public float lowProbabilityPruningThreshold = 0.005f;
        public bool useIncreasingDepth;
        public List<ScoreDepthTuple> increasingDepth; // Use a lower depth early on, and increase it as the game progresses.

        private Expectimax expectimax;
        private ulong state;
        private bool started;

        public override AIType GetAiType() => AIType.ExpectiMax;

        protected override void Start()
        {
            base.Start();
            expectimax = new Expectimax(this);
            StartCoroutine(DoMove());
        }

        private IEnumerator DoMove()
        {
            yield return new WaitForSeconds(0.1f); // Wait for other components to initialize
            TransferState();
            var oneSecondWait = new WaitForSeconds(1f);
            var startTime = DateTime.Now;

            while (true)
            {
                if (!IsRunning())
                {
                    yield return oneSecondWait;
                    continue;
                }

                if (BoardState.IsGameOver())
                {
                    AddRun(new RunResult
                    {
                        StartTime = startTime,
                        EndTime = DateTime.Now,
                        Score = BoardState.Score,
                        MaxTile = BoardState.GetMaxTile()
                    });

                    startTime = DateTime.Now;
                    yield return new WaitForSeconds(AiManager.pauseTimeBetweenRuns);
                    Reset();
                }

                var direction = expectimax.GetBestMove(state);
                state = state.MakeMove(direction);
                MakeMove(direction);
                TransferState();
                yield return new WaitUntil(CanMakeMove);
            }
            // ReSharper disable once IteratorNeverReturns
        }

        private new void Reset()
        {
            state = 0;
            base.Reset();
            TransferState();
        }

        private void TransferState()
        {
            var tiles = new List<byte>();
            BoardState.BoardSmartGrid.ForEach((x, y, arg3) => tiles.Add((byte)Math.Log(BoardState.BoardSmartGrid[y, x].Value, 2)));
            state.SetTiles(tiles);
        }


        public override void SealedUpdate()
        {
        }
        
        [Serializable]
        public struct ScoreDepthTuple
        {
            public int score, depth;
        }
    }
}