using Enemies;
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

        public override void InstallBindings()
        {
            BindSpawning();
        }

        private void BindSpawning()
        {
            Container.BindFactory<Enemy, Enemy.Factory>()
                .FromComponentInNewPrefab(_enemyPrefab)
                .UnderTransform(_enemiesParent);
        }
    }
}
