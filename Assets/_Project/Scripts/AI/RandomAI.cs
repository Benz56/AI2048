using System;

namespace _Project.Scripts.AI
{
    // Not an AI but useful for testing against.
    internal class RandomAI : AbstractAI
    {
        private readonly Random random = new();

        public override void SealedUpdate()
        {
            MakeMove(GetRandomDirection());
        }

        private Direction GetRandomDirection()
        {
            return (Direction)random.Next(0, 4);
        }
    }
}