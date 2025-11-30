using UnityEngine;

namespace Runtime.Enemy.States
{
    public class StunnedState : IEnemyState
    {
        #region PRIVATE_FIELDS

        private readonly DoorEnemyController _enemy;
        private bool _hasResolved;
        private Vector3 _originalScale;

        #endregion

        #region CONSTRUCTORS

        public StunnedState(DoorEnemyController enemy)
        {
            _enemy = enemy;
        }

        #endregion

        #region PUBLIC_METHODS

        public void Enter()
        {
            Debug.Log("[StunnedState] Enemy stunned by torch!");
            _enemy.StopEnemy();
            _hasResolved = false;
            _originalScale = _enemy.Transform.localScale;
        }

        public void Execute()
        {
            if (_hasResolved)
                return;

            if (!_enemy.IsInTorchZone)
            {
                Debug.Log("[StunnedState] Left torch zone - teleporting to player!");
                RestoreScaleInstantly();
                _enemy.TeleportToPlayer();
                _enemy.KillPlayer();
                _enemy.StopEnemy();
                _hasResolved = true;
                
                return;
            }

            _enemy.UpdateTorchTimer();
            UpdateScale();

            if (_enemy.TorchHoldTime >= _enemy.TorchStunDuration)
            {
                Debug.Log("[StunnedState] Held in torch too long - disappearing!");
                _enemy.Disappear();
            }
        }

        public void Exit()
        {
            Debug.Log("[StunnedState] Exiting stunned state.");
            RestoreScaleInstantly();
            _enemy.ResumeEnemy();
        }

        #endregion

        #region PRIVATE_METHODS

        private void UpdateScale()
        {
            float scaleProgress = _enemy.TorchHoldTime / _enemy.TorchStunDuration;
            float targetScale = Mathf.Lerp(1f, 0f, scaleProgress);
            _enemy.Transform.localScale = _originalScale * targetScale;
        }

        private void RestoreScaleInstantly()
        {
            _enemy.Transform.localScale = _originalScale;
        }

        #endregion
    }
}

