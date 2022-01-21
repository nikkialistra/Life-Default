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
    public class EnemyFacade : MonoBehaviour, IPoolable<EnemyType, Vector3, IMemoryPool>, IDisposable
    {
        [Required]
        [SerializeField] private HealthBar _healthBar;
        [Required]
        [SerializeField] private Transform _center;

        private EnemyType _enemyType;

        private bool _died;

        private EntityHealth _health;
        private EnemyAnimator _enemyAnimator;
        private EnemyMeshAgent _enemyMeshAgent;

        private IMemoryPool _pool;

        private void Awake()
        {
            _health = GetComponent<EntityHealth>();
            _enemyAnimator = GetComponent<EnemyAnimator>();
            _enemyMeshAgent = GetComponent<EnemyMeshAgent>();
        }

        public event Action Spawn;
        public event Action HealthChange;
        public event Action Die;

        public event Action DestinationReach;

        public int Health => _health.Health;

        private void OnEnable()
        {
            _health.Die += Dispose;
            _health.HealthChange += OnHealthChange;

            _enemyMeshAgent.DestinationReach += OnDestinationReach;
        }

        private void OnDisable()
        {
            _health.Die -= Dispose;
            _health.HealthChange -= OnHealthChange;

            _enemyMeshAgent.DestinationReach -= OnDestinationReach;
        }

        private void Start()
        {
            if (_pool == null)
            {
                Initialize();
            }
        }

        public void OnSpawned(EnemyType enemyType, Vector3 position, IMemoryPool pool)
        {
            _pool = pool;

            _enemyType = enemyType;
            transform.position = position;

            Initialize();
        }

        public void OnDespawned()
        {
            _pool = null;
        }

        public void Dispose()
        {
            Die?.Invoke();

            _died = true;
            _enemyAnimator.Die(DestroySelf);
        }

        private void Initialize()
        {
            _died = false;

            _health.Initialize();

            _healthBar.SetMaxHealth(_health.MaxHealth);
            _healthBar.SetHealth(_health.Health);

            Spawn?.Invoke();
        }

        private void DestroySelf()
        {
            if (_pool == null)
            {
                Destroy(gameObject);
            }
            else
            {
                _pool.Despawn(this);
            }
        }

        private void OnDestinationReach()
        {
            DestinationReach?.Invoke();
        }

        private void OnHealthChange(int value)
        {
            _healthBar.SetHealth(value);
            HealthChange?.Invoke();
        }

        public class Factory : PlaceholderFactory<EnemyType, Vector3, EnemyFacade> { }
    }
}
