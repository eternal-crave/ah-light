using DG.Tweening;
using NaughtyAttributes;
using Runtime.Enemy;
using UnityEngine;
using UnityEngine.Serialization;
using VContainer;

namespace Runtime.Gameplay
{
    public class Door : MonoBehaviour
    {
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

        private Quaternion _closedRotation;
        private bool _isOpen;
        private EnemyFactory _enemyFactory;

        [Inject]
        private void Construct(EnemyFactory enemyFactory)
        {
            _enemyFactory = enemyFactory;
        }

        private void Awake()
        {
            _closedRotation = doorTransform.rotation;
        }

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

        private void OnDoorOpened()
        {
            if (spawnEnemyOnOpen && _enemyFactory != null)
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

            var enemy = _enemyFactory.Create(doorPoint.position, doorPoint.rotation) as DoorEnemyController;
            if (enemy != null)
            {
                enemy.SetInitialRotation(doorPoint.rotation);                
                enemy.SetPatrolPoints(doorPoint, corridorPoint, wallPoint);
            }
        }

        private void OnDestroy()
        {
            doorTransform.DOKill();
        }
    }
}

