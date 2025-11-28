using Easy.MessageHub;
using Runtime.Enemy.Events;
using Runtime.Enemy.States;
using Runtime.Player;
using UnityEngine;
using UnityEngine.AI;
using VContainer;

namespace Runtime.Enemy
{
    [RequireComponent(typeof(NavMeshAgent))]
    public abstract class EnemyControllerBase : MonoBehaviour
    {
        [Header("Detection")]
        [SerializeField] protected float detectionRange = 10f;
        [SerializeField] protected float killRange = 1.5f;

        [Header("Torch Settings")]
        [SerializeField] protected float torchStunDuration = 3f;

        protected NavMeshAgent _agent;
        protected IEnemyState _currentState;
        protected PlayerBehaviour _player;
        protected IMessageHub _messageHub;

        // State
        protected bool _isInTorchZone;
        protected float _torchHoldTime;

        protected bool _statesCreated = false;
        protected IEnemyPool _pool;

        public NavMeshAgent Agent => _agent;
        public Transform Transform => transform;
        public Transform Player => _player?.transform;
        public float DetectionRange => detectionRange;
        public float KillRange => killRange;
        public bool IsInTorchZone => _isInTorchZone;
        public float TorchHoldTime => _torchHoldTime;
        public float TorchStunDuration => torchStunDuration;

        #region CONSTRUCTORS

        [Inject]
        protected void Construct(PlayerBehaviour player, IMessageHub messageHub)
        {
            _player = player;
            _messageHub = messageHub;
        }

        #endregion

        #region MONO

        protected virtual void Awake()
        {
            _agent = GetComponent<NavMeshAgent>();
        }

        protected virtual void Start()
        {
            // Only create states if not already created (for pooled objects)
            if (!_statesCreated)
            {
                _statesCreated = true;
                CreateStates();
            }
        }

        //could use R3 but it will be overkill i guess
        protected virtual void Update()
        {
            _currentState?.Execute();
        }

        #endregion

        public void SetTorchZone(bool inZone)
        {
            bool wasInZone = _isInTorchZone;
            _isInTorchZone = inZone;

            if (inZone != wasInZone)
                _torchHoldTime = 0f;
        }

        public void UpdateTorchTimer()
        {
            if (_isInTorchZone)
                _torchHoldTime += Time.deltaTime;
        }

        public bool CanDetectPlayer()
        {
            if (_player == null) return false;

            float distance = Vector3.Distance(transform.position, _player.transform.position);
            return distance <= detectionRange;
        }

        public bool IsInKillRange()
        {
            if (_player == null) return false;

            float distance = Vector3.Distance(transform.position, _player.transform.position);
            return distance <= killRange;
        }

        public void KillPlayer()
        {
            Debug.Log("[Enemy] Player killed!");
            _messageHub.Publish(new PlayerKilledEvent(this));
        }

        public void Disappear()
        {
            Disappear(DisappearReason.TorchHeld);
        }

        public void Disappear(DisappearReason reason)
        {
            Debug.Log($"[Enemy] Enemy disappeared! Reason: {reason}");
            _messageHub.Publish(new EnemyDisappearedEvent(this, reason));
            
            ReturnToPool();
        }

        public void TeleportToPlayer()
        {
            if (_player == null) return;

            Vector3 inFrontOfPlayer = _player.transform.position + _player.transform.forward * 1f;
            transform.position = inFrontOfPlayer;
            transform.LookAt(_player.transform);
        }

        public void SetInitialRotation(Quaternion rotation)
        {
            bool wasUpdatingRotation = _agent.updateRotation;
            _agent.updateRotation = false;
            
            transform.rotation = rotation;
            
            _agent.updateRotation = wasUpdatingRotation;
        }

        public void SetInitialPosition(Vector3 position)
        {
            _agent.Warp(position);
        }

        public virtual void ResetForPool()
        {
            // Reset state when taken from pool
            _isInTorchZone = false;
            _torchHoldTime = 0f;
            
            // Reset NavMeshAgent
            _agent.ResetPath();
            _agent.isStopped = false;
            _agent.updateRotation = true;
        }

        public virtual void CleanupForPool()
        {
            // Cleanup when returned to pool
            _currentState?.Exit();
            _currentState = null;
            
            // Reset NavMeshAgent
            _agent.ResetPath();
            _agent.isStopped = true;
        }
        
        protected abstract void CreateStates();

        protected void ChangeState(IEnemyState newState)
        {
            _currentState?.Exit();
            _currentState = newState;
            _currentState?.Enter();
        }

        private void ReturnToPool()
        {
            gameObject.SetActive(false);

            _pool?.Return(this);
        }

        public void SetPool(IEnemyPool pool)
        {
            _pool = pool;
        }

        protected virtual void OnDrawGizmosSelected()
        {
            Gizmos.color = Color.yellow;
            Gizmos.DrawWireSphere(transform.position, detectionRange);

            Gizmos.color = Color.red;
            Gizmos.DrawWireSphere(transform.position, killRange);
        }
    }
}

