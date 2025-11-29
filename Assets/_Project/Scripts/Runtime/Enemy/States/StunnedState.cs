using UnityEngine;

namespace Runtime.Enemy.States
{
    public class StunnedState : IEnemyState
    {
        #region PRIVATE_FIELDS

        private readonly DoorEnemyController _enemy;

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
            _enemy.Agent.isStopped = true;
        }

        public void Execute()
        {
            if (!_enemy.IsInTorchZone)
            {
                Debug.Log("[StunnedState] Left torch zone - teleporting to player!");
                _enemy.TeleportToPlayer();
                _enemy.KillPlayer();
                _enemy.StopEnemy();
                
                return;
            }

            _enemy.UpdateTorchTimer();

            if (_enemy.TorchHoldTime >= _enemy.TorchStunDuration)
            {
                Debug.Log("[StunnedState] Held in torch too long - disappearing!");
                _enemy.Disappear();
            }
        }

        public void Exit()
        {
            Debug.Log("[StunnedState] Exiting stunned state.");
            _enemy.Agent.isStopped = false;
        }

        #endregion
    }
}

