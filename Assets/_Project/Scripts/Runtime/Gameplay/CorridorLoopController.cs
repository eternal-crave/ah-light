using Runtime.Player;
using UnityEngine;
using VContainer;

namespace Runtime.Gameplay
{
    /// <summary>
    /// Manages infinite corridor system. Reuses corridor instances assigned in the editor
    /// by repositioning them as the player progresses forward.
    /// </summary>
    public class CorridorLoopController : MonoBehaviour
    {
        #region SERIALIZED_FIELDS

        [Header("Corridors")]
        [SerializeField] private GameObject[] corridors;
        
        [Header("Settings")]
        [SerializeField] private float corridorLength = 20f;
        [SerializeField] private bool autoCalculateLength = true;
        [SerializeField] private float repositionDistance = 40f;

        #endregion

        #region PRIVATE_FIELDS

        private PlayerBehaviour _player;
        private float[] _initialZPositions;
        private float _furthestZPosition;

        #endregion

        #region PROPERTIES

        public GameObject[] Corridors => corridors;

        #endregion

        #region CONSTRUCTORS

        [Inject]
        private void Construct(PlayerBehaviour player)
        {
            _player = player;
        }

        #endregion

        #region MONO

        private void Start()
        {
            ValidateSettings();
            InitializeCorridors();
        }

        private void Update()
        {
            if (_player == null)
                return;

            UpdateCorridorRepositioning();
        }

        #endregion

        #region PRIVATE_METHODS

        private void ValidateSettings()
        {
            if (corridors == null || corridors.Length == 0)
            {
                Debug.LogError("[CorridorSpawner] No corridors assigned! Please assign corridors in the inspector.");
                enabled = false;
                return;
            }

            foreach (var corridor in corridors)
            {
                if (corridor == null)
                {
                    Debug.LogError("[CorridorSpawner] One or more corridors are null!");
                    enabled = false;
                    return;
                }
            }

            // Auto-calculate corridor length from existing corridor bounds if enabled
            if (autoCalculateLength && corridors.Length > 0 && corridors[0] != null)
            {
                var calculatedLength = CalculateCorridorLength(corridors[0]);
                if (calculatedLength > 0)
                {
                    corridorLength = calculatedLength;
                    Debug.Log($"[CorridorSpawner] Auto-calculated corridor length: {corridorLength}");
                }
            }

            if (corridorLength <= 0)
            {
                Debug.LogWarning("[CorridorSpawner] Corridor length is invalid, using default value of 20.");
                corridorLength = 20f;
            }
        }

        private float CalculateCorridorLength(GameObject corridor)
        {
            if (corridor == null)
                return 0f;

            float length = 0f;

            // Try to get bounds from renderers
            var renderers = corridor.GetComponentsInChildren<Renderer>();
            if (renderers.Length > 0)
            {
                Bounds combinedBounds = renderers[0].bounds;
                foreach (var renderer in renderers)
                {
                    if (renderer != null)
                    {
                        combinedBounds.Encapsulate(renderer.bounds);
                    }
                }
                length = combinedBounds.size.z;
            }
            else
            {
                // Try to get bounds from colliders
                var colliders = corridor.GetComponentsInChildren<Collider>();
                if (colliders.Length > 0)
                {
                    Bounds combinedBounds = colliders[0].bounds;
                    foreach (var collider in colliders)
                    {
                        if (collider != null)
                        {
                            combinedBounds.Encapsulate(collider.bounds);
                        }
                    }
                    length = combinedBounds.size.z;
                }
            }

            return length;
        }

        private void InitializeCorridors()
        {
            // Store initial Z positions to preserve editor setup
            _initialZPositions = new float[corridors.Length];
            _furthestZPosition = float.MinValue;

            for (int i = 0; i < corridors.Length; i++)
            {
                if (corridors[i] != null)
                {
                    var initialZ = corridors[i].transform.position.z;
                    _initialZPositions[i] = initialZ;
                    
                    // Track the furthest corridor position
                    if (initialZ > _furthestZPosition)
                    {
                        _furthestZPosition = initialZ;
                    }
                }
            }

            // Add corridor length to furthest position to get the next spawn position
            _furthestZPosition += corridorLength;
        }

        private void UpdateCorridorRepositioning()
        {
            if (_player == null)
                return;

            var playerZ = _player.transform.position.z;

            // Check each corridor and reposition if needed
            foreach (var corridor in corridors)
            {
                if (corridor == null)
                    continue;

                var corridorZ = corridor.transform.position.z;
                var distanceBehind = playerZ - corridorZ;
                var distanceAhead = corridorZ - playerZ;

                // If corridor is far behind, move it ahead
                if (distanceBehind > repositionDistance)
                {
                    RepositionCorridorAhead(corridor);
                }
                // If corridor is far ahead, move it behind
                else if (distanceAhead > repositionDistance)
                {
                    RepositionCorridorBehind(corridor);
                }
            }
        }

        private void RepositionCorridorAhead(GameObject corridor)
        {
            if (corridor == null || _initialZPositions == null)
                return;

            // Find the index of this corridor
            int corridorIndex = -1;
            for (int i = 0; i < corridors.Length; i++)
            {
                if (corridors[i] == corridor)
                {
                    corridorIndex = i;
                    break;
                }
            }

            if (corridorIndex < 0 || corridorIndex >= _initialZPositions.Length)
                return;

            // Find the current furthest corridor position and its index
            float currentFurthestZ = float.MinValue;
            int furthestIndex = -1;
            for (int i = 0; i < corridors.Length; i++)
            {
                if (corridors[i] != null && corridors[i].transform.position.z > currentFurthestZ)
                {
                    currentFurthestZ = corridors[i].transform.position.z;
                    furthestIndex = i;
                }
            }

            if (furthestIndex < 0)
                return;

            // Find the spacing from the furthest corridor to the next corridor in the initial pattern
            // Sort initial positions to find what comes after the furthest
            float furthestInitialZ = _initialZPositions[furthestIndex];
            float spacing = corridorLength; // Default fallback
            
            // Find the next corridor after the furthest in initial positions
            float nextInitialZ = float.MaxValue;
            foreach (var initialZ in _initialZPositions)
            {
                if (initialZ > furthestInitialZ && initialZ < nextInitialZ)
                {
                    nextInitialZ = initialZ;
                }
            }
            
            // If we found a next corridor, use that spacing
            if (!Mathf.Approximately(nextInitialZ, float.MaxValue))
            {
                spacing = nextInitialZ - furthestInitialZ;
            }
            else
            {
                // No corridor after the furthest, so find spacing from furthest to first (wrap around)
                float firstInitialZ = float.MaxValue;
                foreach (var initialZ in _initialZPositions)
                {
                    if (initialZ < firstInitialZ)
                    {
                        firstInitialZ = initialZ;
                    }
                }
                
                if (!Mathf.Approximately(firstInitialZ, float.MaxValue))
                {
                    // Calculate spacing: from furthest to end of pattern, then from start to first
                    // We'll use the spacing from first to second as the pattern spacing
                    float secondInitialZ = float.MaxValue;
                    foreach (var initialZ in _initialZPositions)
                    {
                        if (initialZ > firstInitialZ && initialZ < secondInitialZ)
                        {
                            secondInitialZ = initialZ;
                        }
                    }
                    
                    if (!Mathf.Approximately(secondInitialZ, float.MaxValue))
                    {
                        spacing = secondInitialZ - firstInitialZ;
                    }
                }
            }

            // Position this corridor after the furthest corridor, maintaining the spacing pattern
            var newZ = currentFurthestZ + spacing;
            var currentPos = corridor.transform.position;
            corridor.transform.position = new Vector3(currentPos.x, currentPos.y, newZ);

            // TODO: In the future, logic for variety (e.g., swapping prefabs) may be implemented here
        }

        private void RepositionCorridorBehind(GameObject corridor)
        {
            if (corridor == null || _initialZPositions == null)
                return;

            // Find the index of this corridor
            int corridorIndex = -1;
            for (int i = 0; i < corridors.Length; i++)
            {
                if (corridors[i] == corridor)
                {
                    corridorIndex = i;
                    break;
                }
            }

            if (corridorIndex < 0 || corridorIndex >= _initialZPositions.Length)
                return;

            // Find the current closest corridor behind the player (furthest back)
            float currentClosestZ = float.MaxValue;
            int closestIndex = -1;
            for (int i = 0; i < corridors.Length; i++)
            {
                if (corridors[i] != null && corridors[i].transform.position.z < currentClosestZ)
                {
                    currentClosestZ = corridors[i].transform.position.z;
                    closestIndex = i;
                }
            }

            if (closestIndex < 0)
                return;

            // Find the spacing from the closest corridor to the previous corridor in the initial pattern
            float closestInitialZ = _initialZPositions[closestIndex];
            float spacing = corridorLength; // Default fallback
            
            // Find the previous corridor before the closest in initial positions
            float prevInitialZ = float.MinValue;
            foreach (var initialZ in _initialZPositions)
            {
                if (initialZ < closestInitialZ && initialZ > prevInitialZ)
                {
                    prevInitialZ = initialZ;
                }
            }
            
            // If we found a previous corridor, use that spacing
            if (!Mathf.Approximately(prevInitialZ, float.MinValue))
            {
                spacing = closestInitialZ - prevInitialZ;
            }
            else
            {
                // No corridor before the closest, so find spacing from last to closest (wrap around)
                float lastInitialZ = float.MinValue;
                foreach (var initialZ in _initialZPositions)
                {
                    if (initialZ > lastInitialZ)
                    {
                        lastInitialZ = initialZ;
                    }
                }
                
                if (!Mathf.Approximately(lastInitialZ, float.MinValue))
                {
                    // Calculate spacing: from last to closest
                    // We'll use the spacing from second-to-last to last as the pattern spacing
                    float secondLastInitialZ = float.MinValue;
                    foreach (var initialZ in _initialZPositions)
                    {
                        if (initialZ < lastInitialZ && initialZ > secondLastInitialZ)
                        {
                            secondLastInitialZ = initialZ;
                        }
                    }
                    
                    if (!Mathf.Approximately(secondLastInitialZ, float.MinValue))
                    {
                        spacing = lastInitialZ - secondLastInitialZ;
                    }
                } 
            }

            // Position this corridor before the closest corridor, maintaining the spacing pattern
            var newZ = currentClosestZ - spacing;
            var currentPos = corridor.transform.position;
            corridor.transform.position = new Vector3(currentPos.x, currentPos.y, newZ);

        }

        #endregion

        #region PUBLIC_METHODS

        /// <summary>
        /// Resets all corridors to their initial positions. Useful for resetting the level.
        /// </summary>
        public void ResetCorridors()
        {
            if (_initialZPositions == null)
                return;

            // Restore all corridors to their initial positions
            for (int i = 0; i < corridors.Length && i < _initialZPositions.Length; i++)
            {
                if (corridors[i] != null)
                {
                    var currentPos = corridors[i].transform.position;
                    corridors[i].transform.position = new Vector3(currentPos.x, currentPos.y, _initialZPositions[i]);
                }
            }

            // Recalculate furthest position
            _furthestZPosition = float.MinValue;
            foreach (var initialZ in _initialZPositions)
            {
                if (initialZ > _furthestZPosition)
                {
                    _furthestZPosition = initialZ;
                }
            }
            _furthestZPosition += corridorLength;
        }

        // /// <summary>
        // /// Gets the index of the next corridor (ahead in Z direction) from the given corridor index.
        // /// </summary>
        // public int GetNextCorridorIndex(int currentIndex)
        // {
        //     if (corridors == null || currentIndex < 0 || currentIndex >= corridors.Length)
        //         return -1;
        //
        //     float currentZ = corridors[currentIndex].transform.position.z;
        //     int nextIndex = -1;
        //     float minDistance = float.MaxValue;
        //
        //     for (int i = 0; i < corridors.Length; i++)
        //     {
        //         if (i == currentIndex || corridors[i] == null)
        //             continue;
        //
        //         float otherZ = corridors[i].transform.position.z;
        //         float distance = otherZ - currentZ;
        //
        //         // Find the closest corridor ahead
        //         if (distance > 0 && distance < minDistance)
        //         {
        //             minDistance = distance;
        //             nextIndex = i;
        //         }
        //     }
        //
        //     return nextIndex;
        // }
        //
        // /// <summary>
        // /// Gets the index of the previous corridor (behind in Z direction) from the given corridor index.
        // /// </summary>
        // public int GetPreviousCorridorIndex(int currentIndex)
        // {
        //     if (corridors == null || currentIndex < 0 || currentIndex >= corridors.Length)
        //         return -1;
        //
        //     float currentZ = corridors[currentIndex].transform.position.z;
        //     int prevIndex = -1;
        //     float minDistance = float.MaxValue;
        //
        //     for (int i = 0; i < corridors.Length; i++)
        //     {
        //         if (i == currentIndex || corridors[i] == null)
        //             continue;
        //
        //         float otherZ = corridors[i].transform.position.z;
        //         float distance = currentZ - otherZ;
        //
        //         // Find the closest corridor behind
        //         if (distance > 0 && distance < minDistance)
        //         {
        //             minDistance = distance;
        //             prevIndex = i;
        //         }
        //     }
        //
        //     return prevIndex;
        // }

        #endregion
    }
}



