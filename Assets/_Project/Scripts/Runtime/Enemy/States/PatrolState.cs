using UnityEngine;

namespace Runtime.Enemy.States
{
    public class PatrolState : IEnemyState
    {
        private readonly DoorEnemyController _enemy;
        private PatrolPhase _phase;
        private float _waitTimer;
        private const float WaitDuration = 3f;
        private const float ArrivalThreshold = 0.5f;

        private enum PatrolPhase
        {
            WalkFromDoor,
            WaitInCorridor,
            GoIntoWall
        }

        public PatrolState(DoorEnemyController enemy)
        {
            _enemy = enemy;
        }

        public void Enter()
        {
            Debug.Log("[PatrolState] Starting patrol from door.");
            _phase = PatrolPhase.WalkFromDoor;
            _waitTimer = 0f;

            // Move to corridor first
            if (_enemy.CorridorPoint != null)
                _enemy.Agent.SetDestination(_enemy.CorridorPoint.position);
        }

        public void Execute()
        {
            switch (_phase)
            {
                case PatrolPhase.WalkFromDoor:
                    HandleWalkFromDoor();
                    break;

                case PatrolPhase.WaitInCorridor:
                    HandleWaitInCorridor();
                    break;

                case PatrolPhase.GoIntoWall:
                    HandleGoIntoWall();
                    break;
            }
        }

        public void Exit()
        {
            Debug.Log("[PatrolState] Exiting patrol.");
        }

        private void HandleWalkFromDoor()
        {
            if (HasReachedDestination())
            {
                Debug.Log("[PatrolState] Reached corridor, starting wait.");
                _phase = PatrolPhase.WaitInCorridor;
                _waitTimer = 0f;
                _enemy.Agent.ResetPath();
            }
        }

        private void HandleWaitInCorridor()
        {
            // Check for player detection while waiting
            if (_enemy.CanDetectPlayer())
            {
                Debug.Log("[PatrolState] Player detected! Transitioning to chase.");
                _enemy.TransitionToChase();
                return;
            }

            // Wait timer
            _waitTimer += Time.deltaTime;
            if (_waitTimer >= WaitDuration)
            {
                Debug.Log("[PatrolState] Wait complete, going into wall.");
                _phase = PatrolPhase.GoIntoWall;

                if (_enemy.WallPoint != null)
                    _enemy.Agent.SetDestination(_enemy.WallPoint.position);
            }
        }

        private void HandleGoIntoWall()
        {
            if (HasReachedDestination())
            {
                Debug.Log("[PatrolState] Reached wall, patrol complete.");
                _enemy.NotifyPatrolComplete();
            }
        }

        private bool HasReachedDestination()
        {
            if (_enemy.Agent.pathPending) return false;
            return _enemy.Agent.remainingDistance <= ArrivalThreshold;
        }
    }
}

