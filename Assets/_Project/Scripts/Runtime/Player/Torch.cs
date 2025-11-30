using System.Collections.Generic;
using Runtime.Enemy;
using UnityEngine;

namespace Runtime.Player
{
    public class Torch : MonoBehaviour
    {
        #region SERIALIZED_FIELDS

        [SerializeField] private Light torchLight;
        [SerializeField] private float detectionRange = 15f;
        [SerializeField] private float sphereCastRadius = 1.5f;
        [SerializeField] private LayerMask enemyLayer;

        #endregion

        #region PRIVATE_FIELDS

        private bool _isOn;
        private Transform _cameraTransform;
        private readonly HashSet<EnemyControllerBase> _trackedEnemies = new();
        private readonly List<EnemyControllerBase> _enemiesToRemove = new();

        #endregion

        #region PROPERTIES

        public bool IsOn => _isOn;

        #endregion

        #region MONO

        private void Start()
        {
            if (torchLight != null)
                torchLight.enabled = _isOn;

            _cameraTransform = Camera.main?.transform;
        }

        private void Update()
        {
            UpdateEnemyDetection();
        }

        #endregion

        #region PUBLIC_METHODS

        public void Toggle()
        {
            _isOn = !_isOn;

            if (torchLight != null)
                torchLight.enabled = _isOn;

            if (!_isOn)
                ClearAllEnemiesFromTorchZone();
        }

        public void SetEnabled(bool enabled)
        {
            _isOn = enabled;

            if (torchLight != null)
                torchLight.enabled = _isOn;

            if (!_isOn)
                ClearAllEnemiesFromTorchZone();
        }

        #endregion

        #region PRIVATE_METHODS

        private void UpdateEnemyDetection()
        {
            CleanupDestroyedEnemies();

            if (!_isOn || _cameraTransform == null)
                return;

            Vector3 origin = _cameraTransform.position;
            Vector3 direction = _cameraTransform.forward;
            
            RaycastHit[] hits = Physics.SphereCastAll(origin, sphereCastRadius, direction, detectionRange, enemyLayer);
            var currentFrameEnemies = new HashSet<EnemyControllerBase>();

            foreach (var hit in hits)
            {
                var enemy = hit.collider.GetComponent<EnemyControllerBase>();
                if (enemy == null || !enemy.gameObject.activeInHierarchy) continue;

                enemy.SetTorchZonePresence(true);
                currentFrameEnemies.Add(enemy);
                _trackedEnemies.Add(enemy);
            }

            foreach (var enemy in _trackedEnemies)
            {
                if (!currentFrameEnemies.Contains(enemy) && enemy != null && enemy.gameObject.activeInHierarchy)
                {
                    enemy.SetTorchZonePresence(false);
                }
            }

            _trackedEnemies.Clear();
            foreach (var enemy in currentFrameEnemies)
                _trackedEnemies.Add(enemy);
        }

        private void CleanupDestroyedEnemies()
        {
            _enemiesToRemove.Clear();

            foreach (var enemy in _trackedEnemies)
            {
                if (enemy == null || !enemy.gameObject.activeInHierarchy)
                    _enemiesToRemove.Add(enemy);
            }

            foreach (var enemy in _enemiesToRemove)
                _trackedEnemies.Remove(enemy);
        }

        private void ClearAllEnemiesFromTorchZone()
        {
            foreach (var enemy in _trackedEnemies)
            {
                if (enemy != null && enemy.gameObject.activeInHierarchy)
                    enemy.SetTorchZonePresence(false);
            }

            _trackedEnemies.Clear();
        }


        #endregion

        #region DEBUG

        private void OnDrawGizmosSelected()
        {
            if (_cameraTransform == null && Camera.main != null)
                _cameraTransform = Camera.main.transform;

            if (_cameraTransform == null || !_isOn) return;

            Vector3 origin = _cameraTransform.position;
            Vector3 direction = _cameraTransform.forward;
            Vector3 endPoint = origin + direction * detectionRange;

            // Draw the spherecast visualization
            Gizmos.color = Color.cyan;
            
            // Draw start sphere
            Gizmos.DrawWireSphere(origin, sphereCastRadius);
            
            // Draw end sphere
            Gizmos.DrawWireSphere(endPoint, sphereCastRadius);
            
            // Draw connecting lines to visualize the cast volume
            Gizmos.DrawLine(origin + Vector3.up * sphereCastRadius, endPoint + Vector3.up * sphereCastRadius);
            Gizmos.DrawLine(origin + Vector3.down * sphereCastRadius, endPoint + Vector3.down * sphereCastRadius);
            Gizmos.DrawLine(origin + Vector3.left * sphereCastRadius, endPoint + Vector3.left * sphereCastRadius);
            Gizmos.DrawLine(origin + Vector3.right * sphereCastRadius, endPoint + Vector3.right * sphereCastRadius);
        }

        #endregion
    }
}
