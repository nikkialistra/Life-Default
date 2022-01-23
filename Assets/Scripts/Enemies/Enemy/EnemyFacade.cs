using System;
using Entities.Entity;
using Entities.Entity.Ancillaries;
using Sirenix.OdinInspector;
using UnityEngine;
using Zenject;

namespace Enemies.Enemy
{
    [RequireComponent(typeof(EntityHealth))]
    [RequireComponent(typeof(EnemyAnimator))]
    [RequireComponent(typeof(EnemyMeshAgent))]
    [RequireComponent(typeof(EnemyBehavior))]
    public class EnemyFacade : MonoBehaviour, IPoolable<EnemyType, Vector3, IMemoryPool>, IDisposable
    {
        [Required]
        [SerializeField] private HealthBar _healthBar;

        [Title("Properties")]
        [SerializeField] private EnemyType _enemyType;

        private bool _died;

        private EntityHealth _health;
        private EnemyAnimator _enemyAnimator;
        private EnemyMeshAgent _enemyMeshAgent;
        private EnemyBehavior _enemyBehavior;

        private IMemoryPool _pool;

        private void Awake()
        {
            _health = GetComponent<EntityHealth>();
            _enemyAnimator = GetComponent<EnemyAnimator>();
            _enemyMeshAgent = GetComponent<EnemyMeshAgent>();
            _enemyBehavior = GetComponent<EnemyBehavior>();
        }

        private void OnEnable()
        {
            _health.HealthChange += OnHealthChange;
            _health.Die += Dispose;
        }

        private void OnDisable()
        {
            _health.HealthChange -= OnHealthChange;
            _health.Die -= Dispose;
        }

        [Button(ButtonSizes.Large)]
        public void TakeDamage(int value)
        {
            if (_died)
            {
                return;
            }

            _health.TakeDamage(value);
        }

        public void OnSpawned(EnemyType enemyType, Vector3 position, IMemoryPool pool)
        {
            _pool = pool;

            _enemyType = enemyType;
            transform.position = position;

            InitializeSelf();
            InitializeComponents();
        }

        public void OnDespawned()
        {
            _pool = null;
        }

        public void Dispose()
        {
            _died = true;

            _enemyMeshAgent.Deactivate();
            _enemyBehavior.StopBehaviorTree();

            _enemyAnimator.Die(DestroySelf);
        }

        private void InitializeSelf()
        {
            _died = false;

            _health.Initialize();

            _healthBar.SetMaxHealth(_health.MaxHealth);
            _healthBar.SetHealth(_health.Health);
        }

        private void InitializeComponents()
        {
            _enemyBehavior.Initialize();
            _enemyBehavior.StartBehaviorTree();
        }

        private void DestroySelf()
        {
            _pool.Despawn(this);
        }

        private void OnHealthChange(int value)
        {
            _healthBar.SetHealth(value);
        }

        public class Factory : PlaceholderFactory<EnemyType, Vector3, EnemyFacade> { }
    }
}
