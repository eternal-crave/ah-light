using UnityEngine;

namespace Runtime.Enemy.States
{
    public class PatrolState : IEnemyState
    {
        #region PRIVATE_FIELDS

        private readonly DoorEnemyController _enemy;
        private PatrolPhase _phase;
        private float _waitTimer;
        private const float WaitDuration = 3f;
        private const float ArrivalThreshold = 0.5f;
        private int _currentPatrolPointIndex;
        private float _patrolPointWaitTime;
        private const float PatrolPointWaitDuration = 1f;

        #endregion

        #region ENUMS

        private enum PatrolPhase
        {
            WalkFromDoor,
            WaitInCorridor,
            GoIntoWall
        }

        #endregion

        #region CONSTRUCTORS

        public PatrolState(DoorEnemyController enemy)
        {
            _enemy = enemy;
        }

        #endregion

        #region PUBLIC_METHODS

        public void Enter()
        {
            Debug.Log("[PatrolState] Starting patrol from door.");
            _phase = PatrolPhase.WalkFromDoor;
            _waitTimer = 0f;
            _currentPatrolPointIndex = 0;
            _patrolPointWaitTime = 0f;

            if (_enemy.DoorPoint != null)
                _enemy.SetInitialPosition(_enemy.DoorPoint.position);

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

        #endregion

        #region PRIVATE_METHODS

        private void HandleWalkFromDoor()
        {
            if (HasReachedDestination())
            {
                Debug.Log("[PatrolState] Reached corridor, starting patrol.");
                _phase = PatrolPhase.WaitInCorridor;
                _waitTimer = 0f;
                _currentPatrolPointIndex = 0;
                _patrolPointWaitTime = 0f;
                
                // If we have patrol points, start patrolling to the first one
                if (HasCorridorPatrolPoints())
                {
                    var patrolPoints = _enemy.CorridorPatrolPoints;
                    if (patrolPoints != null && patrolPoints.Length > 0 && patrolPoints[0] != null)
                    {
                        _enemy.Agent.SetDestination(patrolPoints[0].position);
                    }
                }
                else
                {
                    _enemy.Agent.ResetPath();
                }
            }
        }

        private void HandleWaitInCorridor()
        {
            if (_enemy.CanDetectPlayer())
            {
                Debug.Log("[PatrolState] Player detected! Transitioning to chase.");
                _enemy.TransitionToChase();
                return;
            }

            // Check if we have patrol points to walk between
            if (HasCorridorPatrolPoints())
            {
                HandlePatrolBetweenPoints();
            }
            else
            {
                // Fallback to old behavior: just wait
                HandleWaitBehavior();
            }
        }

        private bool HasCorridorPatrolPoints()
        {
            return _enemy.CorridorPatrolPoints != null && _enemy.CorridorPatrolPoints.Length > 0;
        }

        private void HandlePatrolBetweenPoints()
        {
            var patrolPoints = _enemy.CorridorPatrolPoints;
            
            // Check if we've reached the current patrol point
            if (HasReachedDestination())
            {
                // Wait at the patrol point
                _patrolPointWaitTime += Time.deltaTime;
                if (_patrolPointWaitTime >= PatrolPointWaitDuration)
                {
                    // Move to next patrol point
                    _patrolPointWaitTime = 0f;
                    _currentPatrolPointIndex = (_currentPatrolPointIndex + 1) % patrolPoints.Length;
                    
                    if (patrolPoints[_currentPatrolPointIndex] != null)
                    {
                        _enemy.Agent.SetDestination(patrolPoints[_currentPatrolPointIndex].position);
                    }
                }
            }

            // Check total wait time
            _waitTimer += Time.deltaTime;
            if (_waitTimer >= WaitDuration)
            {
                Debug.Log("[PatrolState] Patrol complete, going into wall.");
                _phase = PatrolPhase.GoIntoWall;
                _enemy.Agent.ResetPath();

                if (_enemy.WallPoint != null)
                    _enemy.Agent.SetDestination(_enemy.WallPoint.position);
            }
        }

        private void HandleWaitBehavior()
        {
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

        #endregion
    }
}

