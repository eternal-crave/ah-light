using Core.StateMachine;
using Helpers;
using Runtime.Gameplay;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace Core.StateMachine.States
{
    /// <summary>
    /// State responsible for restarting/reloading the gameplay scene.
    /// Unloads the current gameplay scene and reloads it.
    /// </summary>
    public class RestartState : BaseState
    {
        #region PRIVATE_FIELDS

        private int _gameplaySceneIndex;

        #endregion

        #region CONSTRUCTORS

        public RestartState(int gameplaySceneIndex = 1)
        {
            _gameplaySceneIndex = gameplaySceneIndex;
        }

        #endregion

        #region PUBLIC_METHODS

        public override void Enter(object payload = default)
        {
            Debug.Log("[RestartState] Restarting gameplay scene...");
            
            UnloadGameplayScene();
        }

        public override void Exit()
        {
            Debug.Log("[RestartState] Restart complete.");
        }

        #endregion

        #region PRIVATE_METHODS

        private void UnloadGameplayScene()
        {
            var gameplayScene = SceneManager.GetSceneByBuildIndex(_gameplaySceneIndex);
            
            if (gameplayScene.IsValid() && gameplayScene.isLoaded)
            {
                SceneManager.UnloadSceneAsync(_gameplaySceneIndex).completed += _ => ReloadGameplayScene();
            }
            else
            {
                ReloadGameplayScene();
            }
        }

        private void ReloadGameplayScene()
        {
            Debug.Log("[RestartState] Reloading gameplay scene...");
            SceneLoader.Load(_gameplaySceneIndex, LoadSceneMode.Additive, OnSceneReloaded);
        }

        private void OnSceneReloaded()
        {
            Debug.Log("[RestartState] Scene reloaded successfully. Transitioning to GameplayState.");
            StateMachine.Enter<GameplayState>();
        }

        #endregion
    }
}

