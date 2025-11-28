using Runtime.Services;
using UnityEngine;
using VContainer;

namespace Runtime.Player
{
    [RequireComponent(typeof(CharacterController))]
    public class FirstPersonController : MonoBehaviour
    {
        [Header("Movement")]
        [SerializeField] private float walkSpeed = 4f;
        [SerializeField] private float sprintSpeed = 7f;
        [SerializeField] private float gravity = -15f;
        [SerializeField] private float jumpHeight = 1.2f;

        [Header("Look")]
        [SerializeField] private Transform cameraTransform;
        [SerializeField] private float lookSensitivity = 1f;
        [SerializeField] private float topClamp = 89f;
        [SerializeField] private float bottomClamp = -89f;

        [Header("Ground Check")]
        [SerializeField] private float groundedOffset = -0.14f;
        [SerializeField] private float groundedRadius = 0.28f;
        [SerializeField] private LayerMask groundLayers;

        private CharacterController _controller;
        private IInputService _inputService;

        private float _verticalVelocity;
        private float _cameraPitch;
        private bool _isGrounded;

        private const float Threshold = 0.01f;

        [Inject]
        private void Construct(IInputService inputService)
        {
            _inputService = inputService;
        }

        private void Start()
        {
            _controller = GetComponent<CharacterController>();
            Cursor.lockState = CursorLockMode.Locked;

            if (cameraTransform == null)
                cameraTransform = Camera.main?.transform;
        }

        private void Update()
        {
            GroundedCheck();
            HandleGravityAndJump();
            HandleMovement();
        }

        private void LateUpdate()
        {
            HandleLook();
        }

        private void GroundedCheck()
        {
            var spherePosition = new Vector3(transform.position.x, transform.position.y - groundedOffset, transform.position.z);
            _isGrounded = Physics.CheckSphere(spherePosition, groundedRadius, groundLayers, QueryTriggerInteraction.Ignore);
        }

        private void HandleGravityAndJump()
        {
            if (_isGrounded)
            {
                if (_verticalVelocity < 0f)
                    _verticalVelocity = -2f;

                if (_inputService.IsJumpPressed)
                    _verticalVelocity = Mathf.Sqrt(jumpHeight * -2f * gravity);
            }

            _verticalVelocity += gravity * Time.deltaTime;
        }

        private void HandleMovement()
        {
            var speed = _inputService.IsSprintPressed ? sprintSpeed : walkSpeed;
            var input = _inputService.MoveInput;

            if (input == Vector2.zero)
                speed = 0f;

            var direction = new Vector3(input.x, 0f, input.y).normalized;
            direction = transform.TransformDirection(direction);

            var velocity = direction * speed + Vector3.up * _verticalVelocity;
            _controller.Move(velocity * Time.deltaTime);
        }

        private void HandleLook()
        {
            var look = _inputService.LookInput;

            if (look.sqrMagnitude < Threshold)
                return;

            // Horizontal rotation (yaw) - rotate the player
            transform.Rotate(Vector3.up * look.x * lookSensitivity);

            // Vertical rotation (pitch) - rotate the camera
            _cameraPitch -= look.y * lookSensitivity;
            _cameraPitch = Mathf.Clamp(_cameraPitch, bottomClamp, topClamp);

            if (cameraTransform != null)
                cameraTransform.localRotation = Quaternion.Euler(_cameraPitch, 0f, 0f);
        }

        private void OnDrawGizmosSelected()
        {
            var transparentGreen = new Color(0f, 1f, 0f, 0.35f);
            var transparentRed = new Color(1f, 0f, 0f, 0.35f);

            Gizmos.color = _isGrounded ? transparentGreen : transparentRed;
            Gizmos.DrawSphere(new Vector3(transform.position.x, transform.position.y - groundedOffset, transform.position.z), groundedRadius);
        }
    }
}

