using Runtime.Gameplay;
using Runtime.LevelInitializers;
using UnityEngine;
using VContainer;
using VContainer.Unity;

namespace Runtime.InjectionBase
{
    public class LevelLifetimeScope : LifetimeScope
    {
        [Header("Scene References")]
        [SerializeField] private LevelController levelController;

        protected override void Configure(IContainerBuilder builder)
        {
            // Controllers
            builder.RegisterComponent(levelController);

            // Entry Point - auto-calls LevelController.Init()
            builder.RegisterEntryPoint<LevelEntryPoint>();
        }
    }
}