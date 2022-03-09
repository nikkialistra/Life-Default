using Entities;
using Entities.Ancillaries;
using Entities.Creature;
using Sirenix.OdinInspector;
using UnityEngine;
using Zenject;

namespace Enemies.Enemy
{
    [RequireComponent(typeof(EntityHealth))]
    [RequireComponent(typeof(EnemyMeshAgent))]
    [RequireComponent(typeof(EnemyBehavior))]
    [RequireComponent(typeof(Entity))]
    public class EnemyFacade : MonoBehaviour
    {
        [Required]
        [SerializeField] private HealthBars _healthBars;
        [Space]
        [Required]
        [SerializeField] private EnemyAnimator _enemyAnimator;

        [Title("Properties")]
        [SerializeField] private EnemyType _enemyType;

        private bool _died;

        private EntityHealth _health;
        private EnemyMeshAgent _enemyMeshAgent;
        private EnemyBehavior _enemyBehavior;

        [Inject]
        public void Construct(EnemyType enemyType, Vector3 position)
        {
            _enemyType = enemyType;
            transform.position = position;
        }

        private void Awake()
        {
            _health = GetComponent<EntityHealth>();
            _enemyMeshAgent = GetComponent<EnemyMeshAgent>();
            _enemyBehavior = GetComponent<EnemyBehavior>();

            Entity = GetComponent<Entity>();
        }

        public Entity Entity { get; private set; }
        public bool Alive => !_died;

        private void Start()
        {
            InitializeSelf();
            InitializeComponents();
        }

        private void OnEnable()
        {
            _health.HealthChange += OnHealthChange;
            _health.Die += Dying;
        }

        private void OnDisable()
        {
            _health.HealthChange -= OnHealthChange;
            _health.Die -= Dying;
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

        private void Dying()
        {
            _died = true;

            _enemyMeshAgent.Deactivate();
            _enemyBehavior.Disable();

            _enemyAnimator.Die(DestroySelf);
        }

        private void InitializeSelf()
        {
            _died = false;

            _health.Initialize();
            
            _healthBars.SetHealth(_health.Vitality);
        }

        private void InitializeComponents()
        {
            _enemyBehavior.Enable();
        }

        private void DestroySelf()
        {
            Destroy(gameObject);
        }

        private void OnHealthChange(float value)
        {
            _healthBars.SetHealth(value);
        }

        public class Factory : PlaceholderFactory<EnemyType, Vector3, EnemyFacade> { }
    }
}
