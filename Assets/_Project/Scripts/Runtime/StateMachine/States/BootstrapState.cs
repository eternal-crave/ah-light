using UnityEngine;

namespace Core.StateMachine.States
{
    /// <summary>
    /// Initial state that runs when the game starts.
    /// Handles bootstrap initialization and transitions to gameplay.
    /// Basically preload
    /// </summary>
    public class BootstrapState : BaseState
    {
        #region PRIVATE_FIELDS

        private readonly int _mainSceneIndex;

        #endregion

        #region CONSTRUCTORS

        public BootstrapState(int mainSceneIndex = 1)
        {
            _mainSceneIndex = mainSceneIndex;
        }

        #endregion

        #region PUBLIC_METHODS

        public override void Enter(object payload = default)
        {
            Debug.Log("[BootstrapState] Initializing game...");
            StateMachine.Enter<LoadingState>(_mainSceneIndex);
        }

        public override void Exit()
        {
            Debug.Log("[BootstrapState] Exiting...");
        }

        #endregion
    }
}
