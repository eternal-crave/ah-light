using UnityEngine;

namespace Runtime.Gameplay
{
    [CreateAssetMenu(fileName = "EnemySpawnSettings", menuName = "Gameplay/Enemy Spawn Settings")]
    public class EnemySpawnSettings : ScriptableObject
    {
        [Header("Enemy Spawning")]
        public bool EnableEnemySpawning = true;
        
        [Range(0f, 1f)] public float EnemySpawnChance = 0.7f;
        public float MinSpawnDistanceFromPlayer = 15f;
        public float MaxSpawnDistanceFromPlayer = 50f;
        
        [Tooltip("Time interval in seconds between spawn checks")]
        public float SpawnCheckInterval = 2f;
    }
}