using UnityEngine;
using VContainer;

namespace Runtime.Player
{
    public class PlayerFollower : MonoBehaviour
    {
        #region SERIALIZED_FIELDS

        [Header("Follow Settings")]
        [SerializeField] private Vector3 relativeOffset = Vector3.zero;

        #endregion

        #region PRIVATE_FIELDS

        private Transform _playerTransform;

        #endregion

        #region CONSTRUCTORS

        [Inject]
        private void Construct(PlayerBehaviour playerBehaviour)
        {
            _playerTransform = playerBehaviour.transform;
        }

        #endregion

        #region MONO

        private void LateUpdate()
        {
            if (_playerTransform == null)
                return;

            UpdatePosition();
        }

        #endregion

        #region PRIVATE_METHODS

        private void UpdatePosition()
        {
            transform.position = _playerTransform.position + relativeOffset;
        }

        #endregion
    }
}

