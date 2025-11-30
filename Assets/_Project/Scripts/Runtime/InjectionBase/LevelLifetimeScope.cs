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
        [SerializeField] private CorridorLoopController corridorLoopController;
        [SerializeField] private EnemySpawnManager enemySpawnManager;

        [Header("Prefabs")]
        [SerializeField] private EnemyControllerBase enemyPrefab;
        
        [Header("Settings")]
        [SerializeField] private EnemySpawnSettings enemySpawnSettings;

        #endregion

        #region PROTECTED_METHODS

        protected override void Configure(IContainerBuilder builder)
        {
            builder.RegisterComponent(levelController);
            builder.RegisterComponent(corridorLoopController);
            builder.RegisterComponent(enemySpawnManager);

            builder.Register<EnemyFactory>(Lifetime.Singleton)
                .WithParameter(enemyPrefab);

            builder.Register<EnemyPool>(Lifetime.Singleton).As<IEnemyPool>();
            
            builder.RegisterInstance(enemySpawnSettings);
            
            builder.Register<EnemySpawnTrigger>(Lifetime.Singleton);
        }

        #endregion
    }
}
