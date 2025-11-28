using UnityEngine;
using VContainer;
using VContainer.Unity;

namespace Runtime.Enemy
{
    public class EnemyFactory
    {
        private readonly IObjectResolver _resolver;
        private readonly EnemyControllerBase _enemyPrefab;

        [Inject]
        public EnemyFactory(IObjectResolver resolver, EnemyControllerBase enemyPrefab)
        {
            _resolver = resolver;
            _enemyPrefab = enemyPrefab;
        }

        public EnemyControllerBase Create(Vector3 position, Quaternion rotation)
        {
            var enemy = _resolver.Instantiate(_enemyPrefab, position, rotation);
            return enemy;
        }

        public EnemyControllerBase Create(Transform spawnPoint)
        {
            return Create(spawnPoint.position, spawnPoint.rotation);
        }

        public EnemyControllerBase Create(Vector3 position)
        {
            return Create(position, Quaternion.identity);
        }
    }
}

