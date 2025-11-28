using Core.StateMachine;
using Core.StateMachine.States;
using UnityEngine;
using UnityEngine.SceneManagement;
using VContainer.Unity;

namespace Core.Gameplay
{
    public class BootstrapEntryPoint : IStartable
    {
        #region PRIVATE_FIELDS

        private readonly GameStateMachine _stateMachine;

        #endregion

        #region CONSTRUCTORS

        public BootstrapEntryPoint(GameStateMachine stateMachine)
        {
            _stateMachine = stateMachine;
        }

        #endregion

        #region PUBLIC_METHODS

        public void Start()
        {
            Debug.Log("[BootstrapEntryPoint] Bootstrap scene initialized. Starting state machine...");
            _stateMachine.Enter<BootstrapState>();
        }

        #endregion

        #region EDITOR

#if UNITY_EDITOR
        private const string BOOTSTRAP_SCENE_NAME = "Bootstrap";

        [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.BeforeSceneLoad)]
        static void AutoLoadBootstrap()
        {
            if (SceneManager.GetActiveScene().name != BOOTSTRAP_SCENE_NAME)
            {
                SceneManager.LoadScene(BOOTSTRAP_SCENE_NAME);
            }
        }
#endif

        #endregion
    }
}
