using System;
using Easy.MessageHub;
using R3;
using Runtime.Enemy.Events;
using Runtime.Gameplay;
using Runtime.Player;
using Runtime.UI;
using UnityEngine;
using VContainer;

namespace Core.StateMachine.States
{
    /// <summary>
    /// State representing active gameplay.
    /// This is where the main game loop runs.
    /// </summary>
    public class GameplayState : BaseState
    {
        #region PRIVATE_FIELDS

        private LevelController _levelController;
        private IDisposable _waitSubscription;
        private DeathUI _deathUI;
        private IMessageHub _messageHub;
        private Guid _restartSubscriptionToken;
        private PlayerBehaviour _playerBehaviour;

        #endregion

        #region CONSTRUCTORS

        [Inject]
        public GameplayState(DeathUI deathUI, IMessageHub messageHub, PlayerBehaviour player)
        {
            _deathUI = deathUI;
            _messageHub = messageHub;
            _playerBehaviour = player;
        }

        #endregion

        #region PUBLIC_METHODS

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

            SubscribeToRestartRequest();
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
            UnsubscribeFromRestartRequest();
            Cleanup();
        }

        #endregion

        #region PRIVATE_METHODS

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
            _playerBehaviour.FirstPersonController.SetMovementEnabled(true);
            HideDeathUI();
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

        private void HideDeathUI()
        {
            if (_deathUI != null)
            {
                _deathUI.Hide();
            }
        }

        private void SubscribeToRestartRequest()
        {
            _restartSubscriptionToken = _messageHub.Subscribe<RestartRequestedEvent>(HandleRestartRequested);
        }

        private void UnsubscribeFromRestartRequest()
        {
            if (_restartSubscriptionToken != Guid.Empty)
            {
                _messageHub.Unsubscribe(_restartSubscriptionToken);
                _restartSubscriptionToken = Guid.Empty;
            }
        }

        private void HandleRestartRequested(RestartRequestedEvent evt)
        {
            Debug.Log("[GameplayState] Restart requested, transitioning to RestartState.");
            StateMachine.Enter<RestartState>();
        }

        #endregion
    }
}
