using UnityEngine;

namespace Runtime.Enemy
{
    public interface IEnemyPool
    {
        EnemyControllerBase Get(Vector3 position, Quaternion rotation);
        EnemyControllerBase Get(Transform spawnPoint);
        void Return(EnemyControllerBase enemy);
    }
}

