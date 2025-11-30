using UnityEngine;
using VContainer;

namespace Runtime.Gameplay
{
    /// <summary>
    /// Manages periodic enemy spawn checks. Periodically evaluates corridors
    /// and triggers enemy spawning based on configured intervals and conditions.
    /// </summary>
    public class EnemySpawnManager : MonoBehaviour
    {
        #region PRIVATE_FIELDS

        private CorridorLoopController _corridorLoopController;
        private EnemySpawnTrigger _enemySpawnTrigger;
        private EnemySpawnSettings _settings;
        private float _lastSpawnCheckTime;

        #endregion

        #region CONSTRUCTORS

        [Inject]
        private void Construct(
            CorridorLoopController corridorLoopController,
            EnemySpawnTrigger enemySpawnTrigger,
            EnemySpawnSettings settings)
        {
            _corridorLoopController = corridorLoopController;
            _enemySpawnTrigger = enemySpawnTrigger;
            _settings = settings;
        }

        #endregion

        #region MONO

        private void Start()
        {
            _lastSpawnCheckTime = Time.time;
        }

        private void Update()
        {
            if (_settings == null || !_settings.EnableEnemySpawning)
                return;

            if (_corridorLoopController == null || _enemySpawnTrigger == null)
                return;

            CheckForSpawnOpportunities();
        }

        #endregion

        #region PRIVATE_METHODS

        private void CheckForSpawnOpportunities()
        {
            float currentTime = Time.time;
            float timeSinceLastCheck = currentTime - _lastSpawnCheckTime;

            if (timeSinceLastCheck < _settings.SpawnCheckInterval)
                return;

            _lastSpawnCheckTime = currentTime;
            EvaluateCorridorsForSpawning();
        }

        private void EvaluateCorridorsForSpawning()
        {
            var corridors = _corridorLoopController.Corridors;
            if (corridors == null || corridors.Length == 0)
                return;

            foreach (var corridor in corridors)
            {
                if (corridor == null)
                    continue;

                _enemySpawnTrigger.TrySpawnEnemiesInCorridor(corridor);
            }
        }

        #endregion
    }
}

