using Runtime.Services;
using UnityEngine;
using VContainer;

namespace Runtime.Player
{
    public class PlayerBehaviour : MonoBehaviour
    {
        #region SERIALIZED_FIELDS

        [Header("Equipment")]
        [SerializeField] private Torch torch;

        #endregion

        #region PRIVATE_FIELDS

        private IInputService _inputService;

        #endregion

        #region CONSTRUCTORS

        [Inject]
        private void Construct(IInputService inputService)
        {
            _inputService = inputService;
        }

        #endregion

        #region MONO

        private void OnEnable()
        {
            _inputService.OnTorchPressed += HandleTorchInput;
        }

        private void OnDisable()
        {
            _inputService.OnTorchPressed -= HandleTorchInput;
        }

        #endregion

        #region PRIVATE_METHODS

        private void HandleTorchInput()
        {
            torch.Toggle();
        }

        #endregion
    }
}
