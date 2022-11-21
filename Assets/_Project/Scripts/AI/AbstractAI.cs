using UnityEngine;

namespace _Project.Scripts.AI
{
    public abstract class AbstractAI : MonoBehaviour
    {
        public AIType aiType;
        private AIManager aiManager;
        protected GameController gameController;
        private BoardState boardState;

        private void Start()
        {
            aiManager = transform.GetComponent<AIManager>();
            gameController = GameObject.Find("Game").GetComponent<GameController>();
            boardState = gameController.BoardState;
        }

        private void FixedUpdate()
        {
            if (aiManager.paused || !IsSelectedAIType() || !CanMakeMove()) return;
            SealedUpdate();
        }

        public bool MakeMove(Direction direction)
        {
            return gameController.MakeMove(direction);
        }

        public bool CanMakeMove()
        {
            return !gameController.Animating();
        }
        
        public bool IsSelectedAIType()
        {
            return aiType == aiManager.selectedAIType;
        }

        public abstract void SealedUpdate();
    }
}