using UnityEngine;

namespace Runtime.Enemy.States
{
    public class ChaseState : IEnemyState
    {
        #region PRIVATE_FIELDS

        private readonly DoorEnemyController _enemy;

        #endregion

        #region CONSTRUCTORS

        public ChaseState(DoorEnemyController enemy)
        {
            _enemy = enemy;
        }

        #endregion

        #region PUBLIC_METHODS

        public void Enter()
        {
            Debug.Log("[ChaseState] Starting chase!");
            _enemy.Agent.isStopped = false;
        }

        public void Execute()
        {
            if (_enemy.IsInTorchZone)
            {
                Debug.Log("[ChaseState] Hit by torch, transitioning to stunned.");
                _enemy.TransitionToStunned();
                return;
            }

            if (_enemy.Player != null)
            {
                _enemy.Agent.SetDestination(_enemy.Player.position);
            }

            if (_enemy.IsInKillRange())
            {
                _enemy.KillPlayer();
            }
        }

        public void Exit()
        {
            Debug.Log("[ChaseState] Stopping chase.");
            _enemy.Agent.ResetPath();
        }

        #endregion
    }
}

