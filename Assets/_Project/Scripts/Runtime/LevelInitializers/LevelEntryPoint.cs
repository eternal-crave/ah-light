using Core.StateMachine;
using Core.StateMachine.States;
using Runtime.Gameplay;
using VContainer;
using VContainer.Unity;

namespace Runtime.LevelInitializers
{
    public class LevelEntryPoint : IStartable
    {
        private readonly LevelController _levelController;
        private readonly GameStateMachine _stateMachine;

        [Inject]
        public LevelEntryPoint(LevelController levelController, GameStateMachine stateMachine)
        {
            _levelController = levelController;
            _stateMachine = stateMachine;
        }

        public void Start()
        {
            // Give LevelController reference to GameplayState
            var gameplayState = _stateMachine.GetState<GameplayState>();
            gameplayState.SetLevelController(_levelController);
        }
    }
}

