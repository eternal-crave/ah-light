using Core.StateMachine;
using Core.StateMachine.States;
using Runtime.Player;
using UnityEngine;
using VContainer;

namespace Runtime.Gameplay
{
    public class LevelController : MonoBehaviour
    {
        #region SERIALIZED_FIELDS

        [Header("References")]
        [SerializeField] private Transform playerStartPoint;

        #endregion

        #region PRIVATE_FIELDS

        private PlayerBehaviour _playerController;
        private GameStateMachine _stateMachine;

        #endregion

        #region PROPERTIES

        public Transform PlayerStartPoint => playerStartPoint;

        #endregion

        #region CONSTRUCTORS

        [Inject]
        private void Construct(PlayerBehaviour playerController, GameStateMachine stateMachine)
        {
            _playerController = playerController;
            _stateMachine = stateMachine;

            var gameplayState = _stateMachine.GetState<GameplayState>();
            gameplayState.SetLevelController(this);
        }

        #endregion

        #region PUBLIC_METHODS

        public void Init()
        {
            SetupPlayer();
        }

        #endregion

        #region PRIVATE_METHODS

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
