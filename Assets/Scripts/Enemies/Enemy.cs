using Common;
using Entities;
using Sirenix.OdinInspector;
using Units;
using Units.Ancillaries;
using Units.Appearance;
using UnityEngine;
using Zenject;
using static Units.Appearance.HumanAppearanceRegistry;

namespace Enemies
{
    [RequireComponent(typeof(UnitVitality))]
    [RequireComponent(typeof(EnemyAnimator))]
    [RequireComponent(typeof(EnemyMeshAgent))]
    [RequireComponent(typeof(EnemyBehavior))]
    [RequireComponent(typeof(Entity))]
    public class Enemy : MonoBehaviour
    {
        [SerializeField] private string _name;
        [SerializeField] private Gender _gender;
        
        [Space]
        [SerializeField] private EnemyType _enemyType;
        
        [Required]
        [SerializeField] private HealthBars _healthBars;
        [Required]
        [SerializeField] private HumanAppearance _humanAppearance;

        private bool _died;

        private UnitVitality _vitality;
        private EnemyAnimator _animator;
        private EnemyMeshAgent _meshAgent;
        private EnemyBehavior _behavior;
        
        private HumanAppearanceRegistry _humanAppearanceRegistry;
        private HumanNames _humanNames;

        [Inject]
        public void Construct(HumanAppearanceRegistry humanAppearanceRegistry , HumanNames humanNames)
        {
            _humanAppearanceRegistry = humanAppearanceRegistry;
            _humanNames = humanNames;
        }

        private void Awake()
        {
            _vitality = GetComponent<UnitVitality>();
            _animator = GetComponent<EnemyAnimator>();
            _meshAgent = GetComponent<EnemyMeshAgent>();
            _behavior = GetComponent<EnemyBehavior>();

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
        
        public void SetAt(Vector3 position)
        {
            transform.position = position;
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
        
        [Button(ButtonSizes.Medium)]
        public void RandomizeAppearance()
        {
            _humanAppearance.RandomizeAppearanceWith(_gender, HumanType.Enemy, _humanAppearanceRegistry);
        }

        private void Dying()
        {
            _died = true;

            _meshAgent.Deactivate();
            _behavior.Disable();

            _animator.Die(DestroySelf);
        }

        private void InitializeSelf()
        {
            _gender = EnumUtils.RandomValue<Gender>();

            if (_name == "")
            { 
                _name = _humanNames.GetRandomNameFor(_gender);
            }
                
            _humanAppearance.RandomizeAppearanceWith(_gender, HumanType.Enemy, _humanAppearanceRegistry);

            _vitality.Initialize();
            
            _healthBars.SetHealth(_vitality.Health);
        }

        private void InitializeComponents()
        {
            _behavior.Enable();
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

        public class Factory : PlaceholderFactory<Enemy> { }
    }
}
