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
        #region SERIALIZED_FIELDS

        [Header("Scene Settings")]
        [SerializeField] private int gameplaySceneIndex = 1;
        
        [Header("Services")]
        [SerializeField] private InputService inputService;

        [Header("Scene References")]
        [SerializeField] private PlayerBehaviour playerBehaviour;

        #endregion

        #region PROTECTED_METHODS

        protected override void Configure(IContainerBuilder builder)
        {
            SetupStateMachine(builder);
            RegisterServices(builder);

            builder.RegisterEntryPoint<BootstrapEntryPoint>().WithParameter(gameplaySceneIndex);
            builder.RegisterComponent(playerBehaviour);
        }

        #endregion

        #region PRIVATE_METHODS

        private void RegisterServices(IContainerBuilder builder)
        {
            builder.RegisterInstance(inputService).As<IInputService>().AsSelf();
            builder.Register<MessageHub>(Lifetime.Singleton).As<IMessageHub>();
        }

        private void SetupStateMachine(IContainerBuilder builder)
        {
            builder.Register<GameStateMachine>(Lifetime.Singleton);

            builder.Register<BootstrapState>(Lifetime.Singleton)
                .WithParameter(gameplaySceneIndex)
                .As<IState>();
            builder.Register<LoadingState>(Lifetime.Singleton).As<IState>();
            builder.Register<GameplayState>(Lifetime.Singleton).As<IState>();
        }

        #endregion
    }
}
