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
        [SerializeField] private Transform wallPoint;

        [Header("Enemy Spawn")]
        [SerializeField] private bool spawnEnemyOnOpen = true;

        #endregion

        #region PRIVATE_FIELDS

        private Quaternion _closedRotation;
        private bool _isOpen;
        private IEnemyPool _enemyPool;

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
            doorTransform.DORotate(_closedRotation.eulerAngles, openDuration)
                .SetEase(openEase);
        }

        #endregion

        #region PRIVATE_METHODS

        private void OnDoorOpened()
        {
            if (spawnEnemyOnOpen && _enemyPool != null)
            {
                SpawnEnemy();
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
                enemy.SetPatrolPoints(doorPoint, corridorPoint, wallPoint);
                enemy.StartActivity();
            }
        }

        #endregion
    }
}
