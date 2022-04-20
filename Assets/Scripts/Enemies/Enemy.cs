using Entities;
using Sirenix.OdinInspector;
using Units;
using Units.Ancillaries;
using UnityEngine;
using Zenject;

namespace Enemies
{
    [RequireComponent(typeof(EntityVitality))]
    [RequireComponent(typeof(EnemyMeshAgent))]
    [RequireComponent(typeof(EnemyBehavior))]
    [RequireComponent(typeof(Entity))]
    public class Enemy : MonoBehaviour
    {
        [Required]
        [SerializeField] private HealthBars _healthBars;
        [Space]
        [Required]
        [SerializeField] private EnemyAnimator _enemyAnimator;

        [Title("Properties")]
        [SerializeField] private EnemyType _enemyType;

        private bool _died;

        private EntityVitality _vitality;
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
            _vitality = GetComponent<EntityVitality>();
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
            _vitality.HealthChange += OnVitalityChange;
            _vitality.Die += Dying;
        }

        private void OnDisable()
        {
            _vitality.HealthChange -= OnVitalityChange;
            _vitality.Die -= Dying;
        }

        [Button(ButtonSizes.Large)]
        public void TakeDamage(int value)
        {
            if (_died)
            {
                return;
            }

            _vitality.TakeDamage(value);
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

            _vitality.Initialize();
            
            _healthBars.SetHealth(_vitality.Health);
        }

        private void InitializeComponents()
        {
            _enemyBehavior.Enable();
        }

        private void DestroySelf()
        {
            Destroy(gameObject);
        }

        private void OnVitalityChange(float vitality, float blood)
        {
            _healthBars.SetHealth(vitality);
            _healthBars.SetRecoverySpeed(blood);
        }

        public class Factory : PlaceholderFactory<EnemyType, Vector3, Enemy> { }
    }
}
