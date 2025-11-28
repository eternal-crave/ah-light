using Runtime.Services;
using UnityEngine;
using VContainer;

namespace Runtime.Player
{
    public class PlayerBehaviour : MonoBehaviour
    {
        [Header("Equipment")]
        [SerializeField] private Torch torch;

        private IInputService _inputService;

        [Inject]
        private void Construct(IInputService inputService)
        {
            _inputService = inputService;
        }

        private void OnEnable()
        {
            _inputService.OnTorchPressed += HandleTorchInput;
        }

        private void OnDisable()
        {
            _inputService.OnTorchPressed -= HandleTorchInput;
        }

        private void HandleTorchInput()
        {
            torch.Toggle();
        }
    }
}

