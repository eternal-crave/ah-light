using Core.Gameplay;
using Core.StateMachine;
using Core.StateMachine.States;
using Easy.MessageHub;
using Runtime.Player;
using Runtime.Services;
using UnityEngine;
using VContainer;
using VContainer.Unity;

namespace Runtime.InjectionBase
{
    public class BootstrapLifetimeScope : LifetimeScope
    {
        [Header("Scene Settings")]
        [SerializeField] private int gameplaySceneIndex = 1;
        
        [Header("Services")]
        [SerializeField] private InputService inputService;

        [Header("Scene References")]
        [SerializeField] private PlayerBehaviour playerBehaviour;

        protected override void Configure(IContainerBuilder builder)
        {
            SetupStateMachine(builder);
            RegisterServices(builder);

            // Register EntryPoint for Bootstrap initialization
            builder.RegisterEntryPoint<BootstrapEntryPoint>().WithParameter(gameplaySceneIndex);
            
            // Register Scene References
            builder.RegisterComponent(playerBehaviour);
        }

        private void RegisterServices(IContainerBuilder builder)
        {
            builder.RegisterInstance(inputService).As<IInputService>().AsSelf();
            
            // Message Hub (Event Bus)
            builder.Register<MessageHub>(Lifetime.Singleton).As<IMessageHub>();
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