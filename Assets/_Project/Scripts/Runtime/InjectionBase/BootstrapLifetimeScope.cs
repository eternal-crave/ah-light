using Core.Gameplay;
using Core.StateMachine;
using Core.StateMachine.States;
using UnityEngine;
using VContainer;
using VContainer.Unity;

namespace Runtime.InjectionBase
{
    public class BootstrapLifetimeScope : LifetimeScope
    {
        [SerializeField] private int gameplaySceneIndex = 1;

        protected override void Configure(IContainerBuilder builder)
        {
            SetupStateMachine(builder);

            // Register EntryPoint for Bootstrap initialization
            builder.RegisterEntryPoint<BootstrapEntryPoint>().WithParameter(gameplaySceneIndex);
        }

        private void SetupStateMachine(IContainerBuilder builder)
        {
            // Register State Machine
            builder.Register<GameStateMachine>(Lifetime.Singleton);

            // Register all game states
            builder.Register<BootstrapState>(Lifetime.Singleton)
                .WithParameter(gameplaySceneIndex)
                .As<IState>();
            builder.Register<LoadingState>(Lifetime.Singleton).As<IState>();
            builder.Register<GameplayState>(Lifetime.Singleton).As<IState>();
        }
    }
}