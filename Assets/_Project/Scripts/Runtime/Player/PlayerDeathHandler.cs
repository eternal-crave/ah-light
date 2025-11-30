using System;
using DG.Tweening;
using Easy.MessageHub;
using Runtime.Enemy.Events;
using Runtime.Services;
using Runtime.UI;
using Helpers;
using UnityEngine;
using VContainer;
using VContainer.Unity;

namespace Runtime.Player
{
    public class PlayerDeathHandler : IStartable, IDisposable
    {
        #region PRIVATE_FIELDS

        private const float youDiedDisplayDuration = 2f;
        private const float deathCountDisplayDuration = 1f;

        private readonly IMessageHub _messageHub;
        private readonly FirstPersonController _firstPersonController;
        private readonly DeathUI _deathUI;
        private readonly IDeathCounterService _deathCounterService;
        private Sequence _deathSequence;

        #endregion

        #region CONSTRUCTORS

        [Inject]
        public PlayerDeathHandler(
            IMessageHub messageHub,
            PlayerBehaviour playerBehaviour,
            DeathUI deathUI,
            IDeathCounterService deathCounterService)
        {
            _messageHub = messageHub;
            _firstPersonController = playerBehaviour.FirstPersonController;
            _deathUI = deathUI;
            _deathCounterService = deathCounterService;
        }

        #endregion

        #region PUBLIC_METHODS

        public void Start()
        {
            _messageHub.SubscribeSafe<PlayerKilledEvent>(_deathUI, HandlePlayerKilled);
            _deathUI.UpdateDeathCount(_deathCounterService.DeathCount);
        }

        public void Dispose()
        {
            _deathSequence?.Kill();
        }

        #endregion

        #region PRIVATE_METHODS

        private void HandlePlayerKilled(PlayerKilledEvent evt)
        {
            Debug.Log("[PlayerDeathHandler] Player killed, starting death sequence.");
            
            DisablePlayerMovement();
            ShowDeathUI();
            StartDeathSequence();
        }

        private void DisablePlayerMovement()
        {
            _firstPersonController.SetMovementEnabled(false);
        }

        private void ShowDeathUI()
        {
            DOVirtual.DelayedCall(0.5f,_deathUI.Show);
        }

        private void StartDeathSequence()
        {
            _deathSequence?.Kill();
            
            _deathSequence = DOTween.Sequence();
            _deathSequence.AppendInterval(youDiedDisplayDuration);
            _deathSequence.AppendCallback(IncrementAndDisplayDeathCount);
            _deathSequence.AppendInterval(deathCountDisplayDuration);
            _deathSequence.AppendCallback(RequestRestart);
        }

        private void RequestRestart()
        {
            Debug.Log("[PlayerDeathHandler] Death sequence complete, requesting restart.");
            _messageHub.Publish(new RestartRequestedEvent());
        }
        
        private void IncrementAndDisplayDeathCount()
        {
            _deathCounterService.IncrementDeathCount();
            _deathUI.UpdateDeathCount(_deathCounterService.DeathCount);
        }

        #endregion
    }
}

