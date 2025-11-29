using UnityEngine;

namespace Runtime.Enemy.States
{
    public class ChaseState : IEnemyState
    {
        #region PRIVATE_FIELDS

        private readonly DoorEnemyController _enemy;
        private bool _hasKilledPlayer;

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
            _hasKilledPlayer = false;
        }

        public void Execute()
        {
            if (_enemy.IsInTorchZone)
            {
                Debug.Log("[ChaseState] Hit by torch, transitioning to stunned.");
                _enemy.TransitionToStunned();
                return;
            }

            if (_hasKilledPlayer)
                return;

            if (_enemy.Player != null)
            {
                _enemy.Agent.SetDestination(_enemy.Player.position);
            }

            if (_enemy.IsInKillRange())
            {
                _enemy.KillPlayer();
                _hasKilledPlayer = true;
                _enemy.Agent.isStopped = true;
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

