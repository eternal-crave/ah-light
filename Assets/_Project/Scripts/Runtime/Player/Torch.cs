using System.Collections.Generic;
using Runtime.Enemy;
using UnityEngine;

namespace Runtime.Player
{
    public class Torch : MonoBehaviour
    {
        [SerializeField] private Light torchLight;
        [SerializeField] private float detectionRange = 15f;
        [SerializeField] private float detectionAngle = 30f;
        [SerializeField] private LayerMask enemyLayer;

        private bool _isOn;
        private Transform _cameraTransform;
        private readonly HashSet<EnemyControllerBase> _trackedEnemies = new();
        private readonly List<EnemyControllerBase> _enemiesToRemove = new();

        public bool IsOn => _isOn;

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

        public void Toggle()
        {
            _isOn = !_isOn;

            if (torchLight != null)
                torchLight.enabled = _isOn;

            // If turned off, notify all tracked enemies they're not in torch zone
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

        private void UpdateEnemyDetection()
        {
            // Clean up destroyed enemies
            CleanupDestroyedEnemies();

            if (!_isOn)
                return;

            // Find all enemies in range
            var colliders = Physics.OverlapSphere(transform.position, detectionRange, enemyLayer);
            var currentFrameEnemies = new HashSet<EnemyControllerBase>();

            foreach (var col in colliders)
            {
                var enemy = col.GetComponent<EnemyControllerBase>();
                if (enemy == null || !enemy.gameObject.activeInHierarchy) continue;

                bool inCone = IsInLightCone(enemy.transform.position);
                enemy.SetTorchZone(inCone);

                if (inCone)
                {
                    currentFrameEnemies.Add(enemy);
                    _trackedEnemies.Add(enemy);
                }
            }

            // Notify enemies that left the torch zone
            foreach (var enemy in _trackedEnemies)
            {
                if (!currentFrameEnemies.Contains(enemy) && enemy != null && enemy.gameObject.activeInHierarchy)
                {
                    enemy.SetTorchZone(false);
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
                    enemy.SetTorchZone(false);
            }

            _trackedEnemies.Clear();
        }

        private bool IsInLightCone(Vector3 targetPosition)
        {
            if (_cameraTransform == null) return false;

            Vector3 directionToTarget = (targetPosition - _cameraTransform.position).normalized;
            float angle = Vector3.Angle(_cameraTransform.forward, directionToTarget);

            return angle <= detectionAngle;
        }

        private void OnDrawGizmosSelected()
        {
            if (_cameraTransform == null && Camera.main != null)
                _cameraTransform = Camera.main.transform;

            if (_cameraTransform == null) return;

            Gizmos.color = Color.yellow;
            Gizmos.DrawWireSphere(transform.position, detectionRange);
        }
    }
}

