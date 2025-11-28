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

        public NavMeshAgent Agent => _agent;
        public Transform Transform => transform;
        public Transform Player => _player?.transform;
        public float DetectionRange => detectionRange;
        public float KillRange => killRange;
        public bool IsInTorchZone => _isInTorchZone;
        public float TorchHoldTime => _torchHoldTime;
        public float TorchStunDuration => torchStunDuration;

        [Inject]
        protected void Construct(PlayerBehaviour player, IMessageHub messageHub)
        {
            _player = player;
            _messageHub = messageHub;
        }

        protected virtual void Awake()
        {
            _agent = GetComponent<NavMeshAgent>();
        }

        protected virtual void Start()
        {
            CreateStates();
        }

        //could use R3 but it will be overkill i guess
        protected virtual void Update()
        {
            _currentState?.Execute();
        }

        protected abstract void CreateStates();

        protected void ChangeState(IEnemyState newState)
        {
            _currentState?.Exit();
            _currentState = newState;
            _currentState?.Enter();
        }

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
            gameObject.SetActive(false);
        }

        public void TeleportToPlayer()
        {
            if (_player == null) return;

            Vector3 inFrontOfPlayer = _player.transform.position + _player.transform.forward * 1f;
            transform.position = inFrontOfPlayer;
            transform.LookAt(_player.transform);
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

