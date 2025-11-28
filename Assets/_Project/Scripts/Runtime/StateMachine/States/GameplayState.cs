using System;
using R3;
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
        private IDisposable _waitSubscription;

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

            _waitSubscription = WaitForLevelController(OnLevelControllerReady);
        }

        /// <summary>
        /// Called when exiting gameplay state.
        /// Use for: cleanup, saving progress, despawning entities,
        /// stopping systems, disabling input, etc.
        /// </summary>
        public override void Exit()
        {
            Debug.Log("[GameplayState] Gameplay ended.");
            Cleanup();
        }

        private IDisposable WaitForLevelController(Action<LevelController> onReady)
        {
            if (_levelController != null)
            {
                onReady?.Invoke(_levelController);
                return null;
            }

            return Observable.EveryUpdate()
                .Where(_ => _levelController != null)
                .Take(1)
                .Subscribe(_ => onReady?.Invoke(_levelController));
        }

        private void OnLevelControllerReady(LevelController levelController)
        {
            DisposeSubscription();
            
            Debug.Log("[GameplayState] LevelController ready, initializing...");
            levelController.Init();
        }

        private void Cleanup()
        {
            DisposeSubscription();
            _levelController = null;
        }

        private void DisposeSubscription()
        {
            _waitSubscription?.Dispose();
            _waitSubscription = null;
        }
    }
}
