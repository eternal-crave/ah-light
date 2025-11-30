using Runtime.Player;
using System.Collections.Generic;
using UnityEngine;
using VContainer;

namespace Runtime.Gameplay
{
    /// <summary>
    /// Handles enemy spawning logic and conditions.
    /// </summary>
    public class EnemySpawnTrigger
    {
        #region PRIVATE_FIELDS

        private readonly PlayerBehaviour _player;
        private readonly HashSet<GameObject> _corridorsWithSpawnedEnemies;
        private EnemySpawnSettings _settings;

        #endregion

        #region CONSTRUCTORS

        [Inject]
        public EnemySpawnTrigger(PlayerBehaviour player, EnemySpawnSettings settings)
        {
            _player = player;
            _settings = settings;
            _corridorsWithSpawnedEnemies = new HashSet<GameObject>();
        }

        #endregion

        #region PUBLIC_METHODS

        public void TrySpawnEnemiesInCorridor(GameObject corridor)
        {
            if (!ShouldSpawnEnemies(corridor))
                return;

            var corridorComponent = corridor.GetComponent<Corridor>();
            if (corridorComponent == null || !corridorComponent.HasDoors)
                return;

            OpenDoorsInCorridor(corridorComponent);
            MarkCorridorAsSpawned(corridor);
        }

        /// <summary>
        /// Resets spawn tracking for a corridor so enemies can spawn again when it's repositioned.
        /// </summary>
        public void ResetCorridorSpawnTracking(GameObject corridor)
        {
            if (corridor != null && _corridorsWithSpawnedEnemies != null)
            {
                _corridorsWithSpawnedEnemies.Remove(corridor);
            }
        }

        /// <summary>
        /// Clears all spawn tracking. Useful when resetting the level.
        /// </summary>
        public void ClearAllSpawnTracking()
        {
            _corridorsWithSpawnedEnemies?.Clear();
        }

        #endregion

        #region PRIVATE_METHODS

        private bool ShouldSpawnEnemies(GameObject corridor)
        {
            if (_settings == null || !_settings.EnableEnemySpawning || corridor == null)
                return false;

            if (_corridorsWithSpawnedEnemies.Contains(corridor))
                return false;

            // Check if corridor is within valid spawn distance from player
            if (_player != null && !IsCorridorWithinSpawnDistance(corridor))
                    return false;

            // Check spawn chance
            if (Random.value > _settings.EnemySpawnChance)
                return false;

            return true;
        }

        private void OpenDoorsInCorridor(Corridor corridor)
        {
            if (_player == null)
                return;

            Vector3 playerPosition = _player.transform.position;
            Vector3 playerForward = _player.transform.forward;

            foreach (var door in corridor.Doors)
            {
                if (door != null && IsDoorAheadOfPlayer(door.transform.position, playerPosition, playerForward))
                {
                    door.Open();
                }
            }
        }

        private bool IsCorridorWithinSpawnDistance(GameObject corridor)
        {
            float distanceToPlayer = corridor.transform.position.z - _player.transform.position.z;
            return distanceToPlayer >= _settings.MinSpawnDistanceFromPlayer && 
                   distanceToPlayer <= _settings.MaxSpawnDistanceFromPlayer;
        }

        private bool IsDoorAheadOfPlayer(Vector3 doorPosition, Vector3 playerPosition, Vector3 playerForward)
        {
            // Calculate direction from player to door
            Vector3 toDoor = (doorPosition - playerPosition).normalized;
            
            // Use dot product to check if door is in front of player's view direction
            // Dot product > 0 means the door is in front of the player's view direction
            return Vector3.Dot(playerForward, toDoor) > 0f;
        }

        private void MarkCorridorAsSpawned(GameObject corridor)
        {
            _corridorsWithSpawnedEnemies.Add(corridor);
        }

        #endregion
    }
}

