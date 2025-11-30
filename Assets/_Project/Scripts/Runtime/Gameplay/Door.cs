using DG.Tweening;
using NaughtyAttributes;
using Runtime.Enemy;
using UnityEngine;
using VContainer;

namespace Runtime.Gameplay
{
    public class Door : MonoBehaviour
    {
        #region SERIALIZED_FIELDS

        [Header("Door Settings")]
        [SerializeField] private float openAngle = 90f;
        [SerializeField] private float openDuration = 1f;
        [SerializeField] private Ease openEase = Ease.OutQuad;

        [Header("Door Transform")]
        [SerializeField] private Transform doorTransform;
        
        [Header("Patrol Points")]
        [SerializeField] private Transform doorPoint;
        [SerializeField] private Transform corridorPoint;
        [SerializeField] private Transform[] corridorPatrolPoints;
        [SerializeField] private Transform wallPoint;

        [Header("Enemy Spawn")]
        [SerializeField] private bool spawnEnemyOnOpen = true;

        [Header("Auto Close Settings")]
        [SerializeField] private bool enableAutoClose = true;
        [SerializeField, Range(0f, 1f)] private float autoCloseChance = 0.5f;
        [SerializeField] private float minOpenDuration = 2f;
        [SerializeField] private float maxOpenDuration = 5f;

        #endregion

        #region PRIVATE_FIELDS

        private Quaternion _closedRotation;
        private bool _isOpen;
        private IEnemyPool _enemyPool;
        private bool _enemySpawned;
        private Tween _autoCloseTween;

        #endregion

        #region CONSTRUCTORS

        [Inject]
        private void Construct(IEnemyPool enemyPool)
        {
            _enemyPool = enemyPool;
        }

        #endregion

        #region MONO

        private void Awake()
        {
            _closedRotation = doorTransform.rotation;
        }

        private void OnDestroy()
        {
            doorTransform.DOKill();
            KillAutoCloseTween();
        }

        #endregion

        #region PUBLIC_METHODS

        [Button]
        public void Open()
        {
            if (_isOpen) return;

            _isOpen = true;
            doorTransform.DORotate(_closedRotation.eulerAngles + Vector3.up * openAngle, openDuration)
                .SetEase(openEase)
                .OnComplete(OnDoorOpened);
        }

        [Button]
        public void Close()
        {
            if (!_isOpen) return;

            _isOpen = false;
            _enemySpawned = false;
            KillAutoCloseTween();
            doorTransform.DORotate(_closedRotation.eulerAngles, openDuration)
                .SetEase(openEase);
        }

        #endregion

        #region PRIVATE_METHODS

        private void OnDoorOpened()
        {
            _enemySpawned = false;
            
            if (spawnEnemyOnOpen && _enemyPool != null)
            {
                SpawnEnemy();
            }
            
            // If no enemy was spawned, check if door should auto-close
            if (!_enemySpawned && enableAutoClose)
            {
                TryAutoClose();
            }
        }

        private void SpawnEnemy()
        {
            if (doorPoint == null)
            {
                Debug.LogWarning($"[Door] Cannot spawn enemy - doorPoint is not set on {gameObject.name}");
                return;
            }

            var enemy = _enemyPool.Get(doorPoint.position, doorPoint.rotation) as DoorEnemyController;
            if (enemy != null)
            {
                // Use the overloaded method if we have corridor patrol points
                if (corridorPatrolPoints != null && corridorPatrolPoints.Length > 0)
                {
                    enemy.SetPatrolPoints(doorPoint, corridorPoint, corridorPatrolPoints, wallPoint);
                }
                else
                {
                    enemy.SetPatrolPoints(doorPoint, corridorPoint, wallPoint);
                }
                enemy.StartActivity();
                _enemySpawned = true;
            }
        }

        private void TryAutoClose()
        {
            // Check random chance to close
            if (Random.value > autoCloseChance)
                return;

            // Kill any existing auto-close tween
            KillAutoCloseTween();

            // Calculate random delay before closing
            float delay = Random.Range(minOpenDuration, maxOpenDuration);
            
            // Schedule door to close after delay
            _autoCloseTween = DOVirtual.DelayedCall(delay, () =>
            {
                if (_isOpen && !_enemySpawned)
                {
                    Close();
                }
                _autoCloseTween = null;
            });
        }

        private void KillAutoCloseTween()
        {
            if (_autoCloseTween != null)
            {
                _autoCloseTween.Kill();
                _autoCloseTween = null;
            }
        }

        #endregion
    }
}
