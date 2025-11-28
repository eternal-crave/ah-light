using UnityEngine;
using UnityEngine.Pool;
using VContainer;

namespace Runtime.Enemy
{
    public class EnemyPool : IEnemyPool
    {
        private readonly EnemyFactory _factory;
        private readonly ObjectPool<EnemyControllerBase> _pool;
        private readonly int _defaultCapacity;
        private readonly int _maxSize;

        [Inject]
        public EnemyPool(EnemyFactory factory)
        {
            _factory = factory;
            _defaultCapacity = 10;
            _maxSize = 50;

            _pool = new ObjectPool<EnemyControllerBase>(
                createFunc: CreateEnemy,
                actionOnGet: OnGetEnemy,
                actionOnRelease: OnReleaseEnemy,
                actionOnDestroy: OnDestroyEnemy,
                collectionCheck: true,
                defaultCapacity: _defaultCapacity,
                maxSize: _maxSize
            );
        }

        public EnemyControllerBase Get(Vector3 position, Quaternion rotation)
        {
            var enemy = _pool.Get();
            enemy.SetInitialPosition(position);
            enemy.SetInitialRotation(rotation);
            return enemy;
        }

        public EnemyControllerBase Get(Transform spawnPoint)
        {
            return Get(spawnPoint.position, spawnPoint.rotation);
        }

        public void Return(EnemyControllerBase enemy)
        {
            if (enemy != null)
            {
                _pool.Release(enemy);
            }
        }

        private EnemyControllerBase CreateEnemy()
        {
            var enemy = _factory.Create(Vector3.zero, Quaternion.identity);
            
            // Store reference to pool for return
            enemy.SetPool(this);

            return enemy;
        }

        private void OnGetEnemy(EnemyControllerBase enemy)
        {
            enemy.ResetForPool();
            enemy.gameObject.SetActive(true);
            
            // Re-initialize enemy after being retrieved from pool
            if (enemy is DoorEnemyController doorEnemy)
            {
                doorEnemy.InitializeFromPool();
            }
        }

        private void OnReleaseEnemy(EnemyControllerBase enemy)
        {
            enemy.CleanupForPool();
            enemy.gameObject.SetActive(false);
        }

        private void OnDestroyEnemy(EnemyControllerBase enemy)
        {
            if (enemy != null)
            {
                Object.Destroy(enemy.gameObject);
            }
        }

        public void Clear()
        {
            _pool.Clear();
        }
    }
}

