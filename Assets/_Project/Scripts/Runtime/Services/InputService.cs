using UnityEngine;
using UnityEngine.InputSystem;

namespace Runtime.Services
{
    public class InputService : MonoBehaviour, IInputService
    {
        [SerializeField] private InputActionAsset playerControls;
        [SerializeField] private string actionMapName = "Player";

        private InputAction _moveAction;
        private InputAction _lookAction;
        private InputAction _sprintAction;
        private InputAction _jumpAction;

        public Vector2 MoveInput { get; private set; }
        public Vector2 LookInput { get; private set; }
        public bool IsSprintPressed { get; private set; }
        public bool IsJumpPressed { get; private set; }

        private void Awake()
        {
            var actionMap = playerControls.FindActionMap(actionMapName);

            _moveAction = actionMap.FindAction("Move");
            _lookAction = actionMap.FindAction("Look");
            _sprintAction = actionMap.FindAction("Sprint");
            _jumpAction = actionMap.FindAction("Jump");

            RegisterInputActions();
        }

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
        }

        private void OnEnable()
        {
            _moveAction.Enable();
            _lookAction.Enable();
            _sprintAction.Enable();
            _jumpAction.Enable();
        }

        private void OnDisable()
        {
            _moveAction.Disable();
            _lookAction.Disable();
            _sprintAction.Disable();
            _jumpAction.Disable();
        }
    }
}

