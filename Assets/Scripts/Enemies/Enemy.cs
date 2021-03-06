using System;
using Common;
using Sirenix.OdinInspector;
using Units;
using Units.Ancillaries;
using Units.Appearance;
using Units.Enums;
using Units.FightBehavior;
using UnityEngine;
using Zenject;
using static Units.Appearance.HumanAppearanceRegistry;

namespace Enemies
{
    [RequireComponent(typeof(Unit))]
    [RequireComponent(typeof(UnitSelection))]
    [RequireComponent(typeof(UnitAttacker))]
    [RequireComponent(typeof(UnitFightBehavior))]
    [RequireComponent(typeof(EnemyAnimator))]
    [RequireComponent(typeof(EnemyMeshAgent))]
    [RequireComponent(typeof(EnemyBehavior))]
    public class Enemy : MonoBehaviour
    {
        public event Action Spawn;
        public event Action HealthChange;
        public event Action<Enemy> EnemyDying;
        public event Action Dying;

        public Unit Unit { get; private set; }

        public FightManner FightManner { get; private set; }

        public string Name => _name;
        public bool Alive => Unit.Alive;

        [SerializeField] private string _name;
        [SerializeField] private Gender _gender;
        [Space]
        [Required]
        [SerializeField] private HumanAppearance _humanAppearance;
        [Space]
        [Required]
        [SerializeField] private GameObject _selectionIndicator;

        private UnitSelection _unitSelection;
        private UnitAttacker _unitAttacker;
        private UnitFightBehavior _unitFightBehavior;

        private EnemyAnimator _animator;
        private EnemyMeshAgent _meshAgent;
        private EnemyBehavior _behavior;

        private HumanAppearanceRegistry _humanAppearanceRegistry;
        private HumanNames _humanNames;

        [Inject]
        public void Construct(HumanAppearanceRegistry humanAppearanceRegistry, HumanNames humanNames)
        {
            _humanAppearanceRegistry = humanAppearanceRegistry;
            _humanNames = humanNames;
        }

        private void Awake()
        {
            Unit = GetComponent<Unit>();
            _unitSelection = GetComponent<UnitSelection>();
            _unitAttacker = GetComponent<UnitAttacker>();
            _unitFightBehavior = GetComponent<UnitFightBehavior>();

            _animator = GetComponent<EnemyAnimator>();
            _meshAgent = GetComponent<EnemyMeshAgent>();
            _behavior = GetComponent<EnemyBehavior>();
        }

        private void Start()
        {
            Initialize();
            ActivateComponents();

            Spawn?.Invoke();
        }

        private void OnEnable()
        {
            Unit.VitalityChange += OnVitalityChange;
            Unit.Dying += OnDying;
        }

        private void OnDisable()
        {
            Unit.VitalityChange -= OnVitalityChange;
            Unit.Dying += OnDying;
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
            Unit.ToggleUnitVisibilityFields();
        }

        private void Initialize()
        {
            _gender = EnumUtils.RandomValue<Gender>();

            if (_name == "")
                _name = _humanNames.GetRandomNameFor(_gender);

            _humanAppearance.RandomizeAppearanceWith(_gender, HumanType.Enemy, _humanAppearanceRegistry);

            FightManner = EnumUtils.RandomValue<FightManner>();

            Unit.Initialize();
        }

        public void Select()
        {
            if (!Unit.Alive) return;

            Unit.Select();

            _unitSelection.Select();
            _selectionIndicator.SetActive(true);
        }

        public void Deselect()
        {
            Unit.Deselect();

            _unitSelection.Deselect();
            _selectionIndicator.SetActive(false);
        }

        public void Die()
        {
            Unit.Die();
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

            _unitFightBehavior.SetManner(FightManner);
        }

        private void DeactivateComponents()
        {
            _behavior.Deactivate();

            _unitSelection.Deactivate();
            _unitAttacker.CoverUnitTarget();
            _unitAttacker.FinalizeAttackingInstantly();

            _meshAgent.Deactivate();
            _behavior.Disable();
        }

        private void DestroySelf()
        {
            Destroy(gameObject);
        }

        private void OnVitalityChange()
        {
            HealthChange?.Invoke();
        }

        public class Factory : PlaceholderFactory<Enemy> { }
    }
}
