using Runtime.Player;
using UnityEngine;
using VContainer;

namespace Runtime.Gameplay
{
    public class LevelController : MonoBehaviour
    {
        #region SERIALIZED_VARIABLES

        [Header("References")]
        [SerializeField] private Transform playerStartPoint;

        #endregion

        #region PRIVATE_VARIABLES

        private PlayerBehaviour _playerController;

        #endregion

        #region PROPERTIES

        public Transform PlayerStartPoint => playerStartPoint;

        #endregion

        #region CONSTRUCTORS

        [Inject]
        private void Construct(PlayerBehaviour playerController)
        {
            _playerController = playerController;
        }

        #endregion

        #region PUBLIC_FUNCTIONS
        
        public void Init()
        {
            SetupPlayer();
        }

        #endregion

        #region PRIVATE_FUNCTIONS

        private void SetupPlayer()
        {
            if (_playerController == null || playerStartPoint == null)
                return;

            _playerController.transform.position = playerStartPoint.position;
            _playerController.transform.rotation = playerStartPoint.rotation;
        }

        #endregion
    }
}

