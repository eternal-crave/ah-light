using Core.StateMachine;
using Core.StateMachine.States;
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
        }

        #endregion

        #region MONO

        private void Start()
        {
            var gameplayState = _stateMachine.GetState<GameplayState>();
            gameplayState.SetLevelController(this);
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

