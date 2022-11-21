using UnityEngine;

namespace _Project.Scripts.AI
{
    public class AIManager : MonoBehaviour
    {
        public AIType aiType;
        
        void Start()
        {
        
        }

        void Update()
        {
        
        }
    }

    public enum AIType
    {
        HumanPlayer, ExpectiMax, NeuralNetwork 
    }
}
