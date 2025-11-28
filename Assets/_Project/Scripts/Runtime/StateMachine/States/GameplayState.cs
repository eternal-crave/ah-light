using Runtime.Gameplay;
using UnityEngine;

namespace Core.StateMachine.States
{
    /// <summary>
    /// State representing active gameplay.
    /// This is where the main game loop runs.
    /// </summary>
    public class GameplayState : BaseState
    {
        private LevelController _levelController;

        public void SetLevelController(LevelController levelController)
        {
            _levelController = levelController;
        }

        /// <summary>
        /// Called when entering gameplay state.
        /// GameplayState controls when level initializes.
        /// </summary>
        public override void Enter(object payload = default)
        {
            Debug.Log("[GameplayState] Gameplay started.");

            _levelController?.Init();
        }

        /// <summary>
        /// Called when exiting gameplay state.
        /// Use for: cleanup, saving progress, despawning entities,
        /// stopping systems, disabling input, etc.
        /// </summary>
        public override void Exit()
        {
            Debug.Log("[GameplayState] Gameplay ended.");
            _levelController = null;
        }
    }
}
