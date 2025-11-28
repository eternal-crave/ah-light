using Runtime.Enemy;
using Runtime.Gameplay;
using UnityEngine;
using VContainer;
using VContainer.Unity;

namespace Runtime.InjectionBase
{
    public class LevelLifetimeScope : LifetimeScope
    {
        #region SERIALIZED_FIELDS

        [Header("Scene References")]
        [SerializeField] private LevelController levelController;

        [Header("Prefabs")]
        [SerializeField] private EnemyControllerBase enemyPrefab;

        #endregion

        #region PROTECTED_METHODS

        protected override void Configure(IContainerBuilder builder)
        {
            builder.RegisterComponent(levelController);

            builder.Register<EnemyFactory>(Lifetime.Singleton)
                .WithParameter(enemyPrefab);

            builder.Register<EnemyPool>(Lifetime.Singleton).As<IEnemyPool>();
        }

        #endregion
    }
}
