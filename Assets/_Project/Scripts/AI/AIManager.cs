using UnityEngine;

namespace _Project.Scripts.AI
{
    public class AIManager : MonoBehaviour
    {
        public bool paused;
        public AIType selectedAIType;

        void Start()
        {
        }

        void Update()
        {
        
        }
    }

    public enum AIType
    {
        HumanPlayer, Random, ExpectiMax, NeuralNetwork 
    }
}
