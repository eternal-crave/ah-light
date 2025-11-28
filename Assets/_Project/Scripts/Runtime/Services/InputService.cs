using System;
using UnityEngine;
using UnityEngine.InputSystem;

namespace Runtime.Services
{
    public class InputService : MonoBehaviour, IInputService
    {
        #region SERIALIZED_FIELDS

        [SerializeField] private InputActionAsset playerControls;
        [SerializeField] private string actionMapName = "Player";

        #endregion

        #region PRIVATE_FIELDS

        private InputAction _moveAction;
        private InputAction _lookAction;
        private InputAction _sprintAction;
        private InputAction _jumpAction;
        private InputAction _torchAction;

        #endregion

        #region PROPERTIES

        public Vector2 MoveInput { get; private set; }
        public Vector2 LookInput { get; private set; }
        public bool IsSprintPressed { get; private set; }
        public bool IsJumpPressed { get; private set; }
        
        public event Action OnTorchPressed;

        #endregion

        #region MONO

        private void Awake()
        {
            var actionMap = playerControls.FindActionMap(actionMapName);

            _moveAction = actionMap.FindAction("Move");
            _lookAction = actionMap.FindAction("Look");
            _sprintAction = actionMap.FindAction("Sprint");
            _jumpAction = actionMap.FindAction("Jump");
            _torchAction = actionMap.FindAction("Torch");

            RegisterInputActions();
        }

        private void OnEnable()
        {
            _moveAction.Enable();
            _lookAction.Enable();
            _sprintAction.Enable();
            _jumpAction.Enable();
            _torchAction.Enable();
        }

        private void OnDisable()
        {
            _moveAction.Disable();
            _lookAction.Disable();
            _sprintAction.Disable();
            _jumpAction.Disable();
            _torchAction.Disable();
        }

        #endregion

        #region PRIVATE_METHODS

        private void RegisterInputActions()
        {
            _moveAction.performed += ctx => MoveInput = ctx.ReadValue<Vector2>();
            _moveAction.canceled += _ => MoveInput = Vector2.zero;

            _lookAction.performed += ctx => LookInput = ctx.ReadValue<Vector2>();
            _lookAction.canceled += _ => LookInput = Vector2.zero;

            _sprintAction.performed += _ => IsSprintPressed = true;
            _sprintAction.canceled += _ => IsSprintPressed = false;

            _jumpAction.performed += _ => IsJumpPressed = true;
            _jumpAction.canceled += _ => IsJumpPressed = false;

            _torchAction.performed += _ => OnTorchPressed?.Invoke();
        }

        #endregion
    }
}
