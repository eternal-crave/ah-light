using UnityEngine;

namespace Runtime.Enemy.States
{
    public class StunnedState : IEnemyState
    {
        private readonly DoorEnemyController _enemy;

        public StunnedState(DoorEnemyController enemy)
        {
            _enemy = enemy;
        }

        public void Enter()
        {
            Debug.Log("[StunnedState] Enemy stunned by torch!");
            _enemy.Agent.isStopped = true;
        }

        public void Execute()
        {
            // If player stops shining torch - instant teleport kill
            if (!_enemy.IsInTorchZone)
            {
                Debug.Log("[StunnedState] Left torch zone - teleporting to player!");
                _enemy.TeleportToPlayer();
                _enemy.KillPlayer();
                return;
            }

            // Count how long player holds torch on enemy
            _enemy.UpdateTorchTimer();

            // If held long enough - enemy disappears
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
    }
}

