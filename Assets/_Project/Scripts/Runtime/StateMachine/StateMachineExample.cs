using Core.StateMachine.States;
using UnityEngine;
using VContainer;

namespace Core.StateMachine
{
    /// <summary>
    /// Example script demonstrating how to use the GameStateMachine.
    /// This shows how to inject and interact with the state machine from any script.
    /// </summary>
    public class StateMachineExample : MonoBehaviour
    {
        private GameStateMachine _stateMachine;

        [Inject]
        public void Init(GameStateMachine stateMachine)
        {
            _stateMachine = stateMachine;
        }

        // Example: Load a specific scene
        public void LoadScene(int sceneIndex)
        {
            _stateMachine.Enter<LoadingState>(sceneIndex);
        }

        // Example: Start gameplay
        public void StartGameplay()
        {
            _stateMachine.Enter<GameplayState>();
        }

        // Example: Get current state info
        public void LogCurrentState()
        {
            var currentState = _stateMachine.ActiveState;
            if (currentState != null)
            {
                Debug.Log($"Current State: {currentState.GetType().Name}");
            }
            else
            {
                Debug.Log("No active state.");
            }
        }
    }
}
