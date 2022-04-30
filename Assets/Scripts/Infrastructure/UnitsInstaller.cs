using Enemies;
using Enemies.Services;
using Sirenix.OdinInspector;
using UnityEngine;
using Zenject;

namespace Infrastructure
{
    public class UnitsInstaller : MonoInstaller
    {
        [Required]
        [SerializeField] private GameObject _enemyPrefab;
        [Required]
        [SerializeField] private Transform _enemiesParent;
        [Required]
        [SerializeField] private EnemyRepository _enemyRepository;

        public override void InstallBindings()
        {
            BindSpawning();

            Container.BindInstance(_enemyRepository);
        }

        private void BindSpawning()
        {
            Container.BindFactory<Enemy, Enemy.Factory>()
                .FromComponentInNewPrefab(_enemyPrefab)
                .UnderTransform(_enemiesParent);
        }
    }
}
