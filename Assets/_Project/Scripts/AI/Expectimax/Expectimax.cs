using System;
using System.Collections.Generic;
using System.Linq;
using _Project.Scripts.AI.Expectimax.Extensions;
using _Project.Scripts.AI.Expectimax.Heuristics;

namespace _Project.Scripts.AI.Expectimax
{
    public class Expectimax
    {
        private readonly ExpectimaxAI expectimaxAI;
        private readonly Direction[] directions = Enum.GetValues(typeof(Direction)).Cast<Direction>().Where(direction => direction != Direction.Undefined).ToArray();
        private Dictionary<ulong, double> stateCache; // Anything over search depth 5 is extremely slow without this.
        private int searchDepth;

        public Expectimax(ExpectimaxAI expectimaxAI)
        {
            this.expectimaxAI = expectimaxAI;
        }

        public Direction GetBestMove(ulong state)
        {
            ResetMoveData();

            var best = (direction: Direction.Undefined, score: double.MinValue);
            foreach (var direction in directions) EvaluateDirection(direction);
            if (best.direction == Direction.Undefined) throw new InvalidOperationException();
            return best.direction;

            void EvaluateDirection(Direction direction)
            {
                var moveState = state.MakeMove(direction);
                if (moveState == state) return;
                var score = GetNodeScore(moveState, 1, 1d);
                best = score <= best.score ? best : (direction, score);
            }

            void ResetMoveData()
            {
                stateCache = new Dictionary<ulong, double>();
                SetDepth(state);
            }
        }

        private void SetDepth(ulong state)
        {
            if (expectimaxAI.useIncreasingDepth)
            {
                var score = state.GetScore();
                for (var index = expectimaxAI.increasingDepth.Count - 1; index >= 0; index--)
                {
                    var scoreDepthTuple = expectimaxAI.increasingDepth[index];
                    if (score < scoreDepthTuple.score) continue;
                    searchDepth = scoreDepthTuple.depth;
                    break;
                }
            }
            else searchDepth = expectimaxAI.searchDepth;
        }

        private double GetNodeScore(ulong state, int depth, double probability)
        {
            if (depth > searchDepth) return state.GetStateHeuristic();
            if (state.IsGameOver()) return state.GetStateHeuristic(true);

            var prune = expectimaxAI.lowProbabilityPruningThreshold > 0 && probability < expectimaxAI.lowProbabilityPruningThreshold;
            return prune ? state.GetStateHeuristic() : TryGetCachedStateScore(state, depth, probability);
        }

        private double TryGetCachedStateScore(ulong state, int depth, double probability)
        {
            if (stateCache.TryGetValue(state, out var cachedStateScore)) return cachedStateScore;
            var everyOther = depth % 2 == 0;
            var score = everyOther ? GetBestMoveScore(state, depth, probability) : GetRandomAverageScore(state, depth, probability);
            stateCache.Add(state, score);
            return score;
        }

        private double GetRandomAverageScore(ulong state, int depth, double probability)
        {
            const double twoProb = 0.9d, fourProb = 0.1d;
            var score = 0d;
            var empty = 0;
            var sCopy = state;

            for (var i = 0; i < 16; i++)
            {
                if ((sCopy & 0xF) == 0)
                {
                    empty++;
                    score += GetNodeScore(state | ((ulong)0x1 << (i * 4)), depth + 1, probability * twoProb) * twoProb + GetNodeScore(state | ((ulong)0x2 << (i * 4)), depth + 1, probability * fourProb) * fourProb;
                }
                sCopy >>= 4;
            }

            if (empty == 0) throw new InvalidOperationException();

            var avgScore = score / empty;
            return avgScore;
        }

        private double GetBestMoveScore(ulong state, int depth, double probability)
        {
            var best = double.MinValue;
            // ReSharper disable once LoopCanBeConvertedToQuery
            // Linq destroys performance.
            foreach (var direction in directions)
            {
                var next = state.MakeMove(direction);
                if (next == state) continue;

                var score = GetNodeScore(next, depth + 1, probability);
                if (score > best) best = score;
            }

            return best;
        }
    }
}