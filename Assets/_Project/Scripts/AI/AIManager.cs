using UnityEngine;

namespace _Project.Scripts.AI
{
    public class AIManager : MonoBehaviour
    {
        public bool paused;
        [Range(0, 10)]
        public float pauseTimeBetweenRuns = 2f;
        [Range(1, 1000)]
        public int terminateAfterRuns = 100;
        public AIType selectedAIType;
    }

    public enum AIType
    {
        HumanPlayer, Random, ExpectiMax 
    }
}
