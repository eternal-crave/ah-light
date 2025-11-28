using UnityEngine;

namespace Runtime.Enemy.States
{
    public class ChaseState : IEnemyState
    {
        private readonly DoorEnemyController _enemy;

        public ChaseState(DoorEnemyController enemy)
        {
            _enemy = enemy;
        }

        public void Enter()
        {
            Debug.Log("[ChaseState] Starting chase!");
            _enemy.Agent.isStopped = false;
        }

        public void Execute()
        {
            // Check if player is using torch on enemy
            if (_enemy.IsInTorchZone)
            {
                Debug.Log("[ChaseState] Hit by torch, transitioning to stunned.");
                _enemy.TransitionToStunned();
                return;
            }

            // Chase player
            if (_enemy.Player != null)
            {
                _enemy.Agent.SetDestination(_enemy.Player.position);
            }

            // Kill if in range
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
    }
}

