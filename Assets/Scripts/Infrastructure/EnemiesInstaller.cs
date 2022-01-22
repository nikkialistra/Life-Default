using Enemies.Enemy;
using Sirenix.OdinInspector;
using UnityEngine;
using Zenject;

namespace Infrastructure
{
    public class EnemiesInstaller : MonoInstaller
    {
        [Required]
        [SerializeField] private GameObject _enemyPrefab;
        [Required]
        [SerializeField] private int _enemyPoolSize;
        [Required]
        [SerializeField] private Transform _enemiesParent;

        public override void InstallBindings()
        {
            BindEnemySpawning();
        }

        private void BindEnemySpawning()
        {
            Container.BindFactory<EnemyType, Vector3, EnemyFacade, EnemyFacade.Factory>()
                .FromPoolableMemoryPool<EnemyType, Vector3, EnemyFacade, EnemyFacadePool>(pool => pool
                    .WithInitialSize(_enemyPoolSize)
                    .FromComponentInNewPrefab(_enemyPrefab)
                    .UnderTransform(_enemiesParent));
        }

        private class EnemyFacadePool : MonoPoolableMemoryPool<EnemyType, Vector3, IMemoryPool, EnemyFacade> { }
    }
}
