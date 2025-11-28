using Runtime.Enemy.Events;
using Runtime.Enemy.States;
using UnityEngine;

namespace Runtime.Enemy
{
    public class DoorEnemyController : EnemyControllerBase
    {
        [Header("Patrol Points")] [SerializeField]
        private Transform doorPoint;

        [SerializeField] private Transform corridorPoint;
        [SerializeField] private Transform wallPoint;

        // States
        private PatrolState _patrolState;
        private ChaseState _chaseState;
        private StunnedState _stunnedState;

        public Transform DoorPoint => doorPoint;
        public Transform CorridorPoint => corridorPoint;
        public Transform WallPoint => wallPoint;

        public void SetPatrolPoints(Transform door, Transform corridor, Transform wall)
        {
            doorPoint = door;
            corridorPoint = corridor;
            wallPoint = wall;
        }

        protected override void CreateStates()
        {
            _patrolState = new PatrolState(this);
            _chaseState = new ChaseState(this);
            _stunnedState = new StunnedState(this);
        }

        public override void ResetForPool()
        {
            base.ResetForPool();

            // States are kept, just reset current state
            _currentState = null;
        }

        public void InitializeFromPool()
        {
            // Ensure states are created
            if (_patrolState == null)
            {
                CreateStates();
            }
        }

        public void TransitionToChase()
        {
            ChangeState(_chaseState);
        }

        public void TransitionToStunned()
        {
            ChangeState(_stunnedState);
        }

        public void TransitionToPatrol()
        {
            ChangeState(_patrolState);
        }

        public void NotifyPatrolComplete()
        {
            Disappear(DisappearReason.PatrolComplete);
        }


    }
}


