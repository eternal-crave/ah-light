using UnityEngine;

namespace Runtime.Player
{
    public class Torch : MonoBehaviour
    {
        [SerializeField] private Light torchLight;
        
        private bool _isOn;

        public bool IsOn => _isOn;

        private void Start()
        {
            if (torchLight != null)
                torchLight.enabled = _isOn;
        }

        public void Toggle()
        {
            _isOn = !_isOn;
            
            if (torchLight != null)
                torchLight.enabled = _isOn;
        }

        public void SetEnabled(bool enabled)
        {
            _isOn = enabled;
            
            if (torchLight != null)
                torchLight.enabled = _isOn;
        }
    }
}

