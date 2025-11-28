using Runtime.Enemy;
using Runtime.Gameplay;
using UnityEngine;
using VContainer;
using VContainer.Unity;

namespace Runtime.InjectionBase
{
    public class LevelLifetimeScope : LifetimeScope
    {
        [Header("Scene References")]
        [SerializeField] private LevelController levelController;

        [Header("Prefabs")]
        [SerializeField] private EnemyControllerBase enemyPrefab;

        protected override void Configure(IContainerBuilder builder)
        {
            // Controllers
            builder.RegisterComponent(levelController);

            // Factories
            builder.Register<EnemyFactory>(Lifetime.Singleton)
                .WithParameter(enemyPrefab);
        }
    }
}