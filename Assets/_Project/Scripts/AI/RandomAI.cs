using System;
using System.Linq;
using UnityEngine;
using Random = System.Random;

namespace _Project.Scripts.AI
{
    // Not an AI but useful for testing against.
    internal class RandomAI : AbstractAI
    {
        private readonly Random random = new();
        private DateTime startTime = DateTime.Now;

        public override void SealedUpdate()
        {
            if (BoardState.IsGameOver())
            {
                AddRun(new RunResult
                {
                    StartTime = startTime,
                    EndTime = DateTime.Now,
                    Score = BoardState.Score,
                    MaxTile = BoardState.GetMaxTile()
                });
                Debug.Log(Runs.Last().EndTime - Runs.Last().StartTime);
                startTime = DateTime.Now;
                Reset();
            }

            MakeMove(GetRandomDirection());
        }

        private Direction GetRandomDirection()
        {
            return (Direction) random.Next(1, 5);
        }

        public override AIType GetAiType() => AIType.Random;
    }
}