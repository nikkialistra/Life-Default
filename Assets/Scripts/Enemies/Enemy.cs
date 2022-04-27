using System;
using Common;
using Sirenix.OdinInspector;
using Units;
using Units.Appearance;
using Units.Enums;
using UnityEngine;
using Zenject;
using static Units.Appearance.HumanAppearanceRegistry;

namespace Enemies
{
    [RequireComponent(typeof(Unit))]
    [RequireComponent(typeof(UnitSelection))]
    [RequireComponent(typeof(EnemyAnimator))]
    [RequireComponent(typeof(EnemyMeshAgent))]
    [RequireComponent(typeof(EnemyBehavior))]
    public class Enemy : MonoBehaviour
    {
        [SerializeField] private string _name;
        [SerializeField] private Gender _gender;
        [Space]
        [Required]
        [SerializeField] private HumanAppearance _humanAppearance;
        [Space]
        [Required]
        [SerializeField] private GameObject _selectionIndicator;

        private Unit _unit;
        private UnitSelection _unitSelection;

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
            _unit = GetComponent<Unit>();
            _unitSelection = GetComponent<UnitSelection>();

            _animator = GetComponent<EnemyAnimator>();
            _meshAgent = GetComponent<EnemyMeshAgent>();
            _behavior = GetComponent<EnemyBehavior>();
        }
        
        public event Action HealthChange;
        public event Action<Enemy> EnemyDying;
        public event Action Dying;
        
        public Unit Unit => _unit;

        public string Name => _name;
        public bool Alive => _unit.Alive;

        private void Start()
        {
            Initialize();
            InitializeComponents();
        }

        private void OnEnable()
        {
            _unit.HealthChange += OnHealthChange;
            _unit.Dying += OnDying;

            _unitSelection.Selected += Select;
            _unitSelection.Deselected += Deselect;
        }

        private void OnDisable()
        {
            _unit.HealthChange -= OnHealthChange;
            _unit.Dying += OnDying;
            
            _unitSelection.Selected -= Select;
            _unitSelection.Deselected -= Deselect;
        }

        public void SetAt(Vector3 position)
        {
            transform.position = position;
        }

        [Button(ButtonSizes.Medium)]
        public void RandomizeAppearance()
        {
            _humanAppearance.RandomizeAppearanceWith(_gender, HumanType.Enemy, _humanAppearanceRegistry);
        }

        public void ToggleUnitFieldOfView()
        {
            _unit.ToggleUnitVisibilityFields();
        }

        private void Initialize()
        {
            _gender = EnumUtils.RandomValue<Gender>();

            if (_name == "")
            { 
                _name = _humanNames.GetRandomNameFor(_gender);
            }
                
            _humanAppearance.RandomizeAppearanceWith(_gender, HumanType.Enemy, _humanAppearanceRegistry);

            _unit.Initialize();
        }

        public void Select()
        {
            if (!_unit.Alive)
            {
                return;
            }
            
            _unit.Select();

            _selectionIndicator.SetActive(true);
        }

        public void Deselect()
        {
            _unit.Deselect();
            
            _selectionIndicator.SetActive(false);
        }

        public void Die()
        {
            _unit.Die();
        }

        private void OnDying()
        {
            _meshAgent.Deactivate();
            _behavior.Disable();
            
            Deselect();

            EnemyDying?.Invoke(this);
            Dying?.Invoke();
            
            _animator.Die(DestroySelf);
        }

        private void InitializeComponents()
        {
            _unitSelection.Activate();
            _behavior.Enable();
        }

        private void DestroySelf()
        {
            Destroy(gameObject);
        }

        private void OnHealthChange()
        {
            HealthChange?.Invoke();
        }

        public class Factory : PlaceholderFactory<Enemy> { }
    }
}
