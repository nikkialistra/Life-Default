using Enemies.Services;
using UnityEngine;
using Zenject;

namespace Enemies
{
    [RequireComponent(typeof(Enemy))]
    public class EnemyCounter : MonoBehaviour
    {
        private Enemy _enemy;

        private EnemyRepository _enemyRepository;

        [Inject]
        public void Construct(EnemyRepository enemyRepository)
        {
            _enemyRepository = enemyRepository;
        }

        private void Awake()
        {
            _enemy = GetComponent<Enemy>();
        }

        private void OnEnable()
        {
            _enemy.Spawn += OnSpawn;
            _enemy.Dying += OnDying;
        }

        private void OnDisable()
        {
            _enemy.Spawn -= OnSpawn;
            _enemy.Dying -= OnDying;
        }

        private void OnSpawn()
        {
            _enemyRepository.AddEnemy(_enemy);
        }

        private void OnDying()
        {
            _enemyRepository.RemoveEnemy(_enemy);
        }
    }
}
