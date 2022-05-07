using System;
using Common;
using Sirenix.OdinInspector;
using Units;
using Units.Ancillaries;
using Units.Appearance;
using Units.Enums;
using UnityEngine;
using Zenject;
using static Units.Appearance.HumanAppearanceRegistry;

namespace Enemies
{
    [RequireComponent(typeof(Unit))]
    [RequireComponent(typeof(UnitSelection))]
    [RequireComponent(typeof(UnitAttacker))]
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
        private UnitAttacker _unitAttacker;

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
            _unitAttacker = GetComponent<UnitAttacker>();

            _animator = GetComponent<EnemyAnimator>();
            _meshAgent = GetComponent<EnemyMeshAgent>();
            _behavior = GetComponent<EnemyBehavior>();
        }
        
        public event Action Spawn;
        public event Action HealthChange;
        public event Action<Enemy> EnemyDying;
        public event Action Dying;
        
        public Unit Unit => _unit;

        public string Name => _name;
        public bool Alive => _unit.Alive;

        private void Start()
        {
            Initialize();
            ActivateComponents();
            
            Spawn?.Invoke();
        }

        private void OnEnable()
        {
            _unit.HealthChange += OnHealthChange;
            _unit.Dying += OnDying;
        }

        private void OnDisable()
        {
            _unit.HealthChange -= OnHealthChange;
            _unit.Dying += OnDying;
        }

        public void SetAt(Vector3 position)
        {
            transform.position = position;
        }

        public void SetAt(Vector3 position, Quaternion rotation)
        {
            transform.position = position;
            transform.rotation = rotation;
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

            _unitSelection.Select();
            _selectionIndicator.SetActive(true);
        }

        public void Deselect()
        {
            _unit.Deselect();
            
            _unitSelection.Deselect();
            _selectionIndicator.SetActive(false);
        }

        public void Die()
        {
            _unit.Die();
        }

        private void OnDying()
        {
            DeactivateComponents();

            Deselect();

            EnemyDying?.Invoke(this);
            Dying?.Invoke();
            
            _animator.Die(DestroySelf);
        }

        private void ActivateComponents()
        {
            _behavior.Activate();
            
            _unitSelection.Activate();
            _behavior.Enable();
        }

        private void DeactivateComponents()
        {
            _behavior.Deactivate();
            
            _unitSelection.Deactivate();
            _unitAttacker.FinalizeAttackingInstantly();
            
            _meshAgent.Deactivate();
            _behavior.Disable();
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
