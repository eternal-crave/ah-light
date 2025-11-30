using NaughtyAttributes;
using UnityEngine;

namespace Runtime.Gameplay
{
    /// <summary>
    /// Represents a corridor segment that can contain doors for enemy spawning.
    /// </summary>
    public class Corridor : MonoBehaviour
    {
        #region SERIALIZED_FIELDS

        [Header("Doors")]
        [SerializeField] private Door[] doors;

        #endregion

        #region PROPERTIES

        public Door[] Doors => doors;

        public bool HasDoors => doors != null && doors.Length > 0;

        #endregion

        #region EDITOR

#if UNITY_EDITOR

        [Button]
        private void FindDoorsInChildren()
        {
            doors = GetComponentsInChildren<Door>();
            UnityEditor.EditorUtility.SetDirty(this);
        }

#endif

        #endregion
    }
}

