﻿using Enemies.Enemy;
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
        [SerializeField] private Transform _enemiesParent;

        public override void InstallBindings()
        {
            BindEnemySpawning();
        }

        private void BindEnemySpawning()
        {
            Container.BindFactory<EnemyType, Vector3, EnemyFacade, EnemyFacade.Factory>()
                .FromComponentInNewPrefab(_enemyPrefab)
                .UnderTransform(_enemiesParent);
        }
    }
}
