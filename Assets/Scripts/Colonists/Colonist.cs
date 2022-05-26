using System;
using System.Collections.Generic;
using Common;
using ResourceManagement;
using Sirenix.OdinInspector;
using Units;
using Units.Ancillaries;
using Units.Appearance;
using Units.Enums;
using Units.Traits;
using UnityEngine;
using Zenject;
using static Units.Appearance.HumanAppearanceRegistry;

namespace Colonists
{
    [RequireComponent(typeof(Unit))]
    [RequireComponent(typeof(UnitSelection))]
    [RequireComponent(typeof(UnitAttacker))]
    [RequireComponent(typeof(ColonistStats))]
    [RequireComponent(typeof(ColonistTraits))]
    [RequireComponent(typeof(ColonistAnimator))]
    [RequireComponent(typeof(ColonistMeshAgent))]
    [RequireComponent(typeof(ColonistGatherer))]
    [RequireComponent(typeof(ColonistBehavior))]
    public class Colonist : MonoBehaviour
    {
        [SerializeField] private string _name;
        [SerializeField] private Gender _gender;
        [Space]
        [Required]
        [SerializeField] private HumanAppearance _humanAppearance;
        [Space]
        [Required]
        [SerializeField] private GameObject _selectionIndicator;
        [Required]
        [SerializeField] private Transform _center;

        private Unit _unit;
        private UnitSelection _unitSelection;
        private UnitAttacker _unitAttacker;

        private ColonistStats _colonistStats;
        private ColonistTraits _colonistTraits;
        private ColonistAnimator _colonistAnimator;
        private ColonistMeshAgent _colonistMeshAgent;
        private ColonistGatherer _colonistGatherer;
        private ColonistBehavior _colonistBehavior;

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

            _colonistStats = GetComponent<ColonistStats>();
            _colonistTraits = GetComponent<ColonistTraits>();
            _colonistAnimator = GetComponent<ColonistAnimator>();
            _colonistMeshAgent = GetComponent<ColonistMeshAgent>();
            _colonistGatherer = GetComponent<ColonistGatherer>();
            _colonistBehavior = GetComponent<ColonistBehavior>();
        }

        public event Action Spawn;
        public event Action VitalityChange;
        public event Action Dying;
        public event Action<Colonist> ColonistDying;
        
        public event Action<string> NameChange;

        public event Action TraitsChange;

        public Unit Unit => _unit;

        public bool Alive => _unit.Alive;
        
        public string Name
        {
            get => _name;
            set
            {
                _name = value;
                NameChange?.Invoke(_name);
            }
        }

        public IReadOnlyList<Trait> Traits => _colonistTraits.Traits;

        public Vector3 Center => _center.position;


        private void Start()
        {
            Initialize();
            ActivateComponents();

            Spawn?.Invoke();
        }

        private void OnEnable()
        {
            _unit.VitalityChange += OnVitalityChange;
            _unit.Dying += OnDying;
        }

        private void OnDisable()
        {
            _unit.VitalityChange -= OnVitalityChange;
            _unit.Dying -= OnDying;
        }

        private void OnDestroy()
        {
            UnbindStatsFromComponents();
        }

        public void SetAt(Vector3 position)
        {
            transform.position = position;
        }

        [Button(ButtonSizes.Medium)]
        public void RandomizeAppearance()
        {
            _humanAppearance.RandomizeAppearanceWith(_gender, HumanType.Colonist, _humanAppearanceRegistry);
        }
        
        [Button(ButtonSizes.Medium)]
        public void AddTrait(Trait trait)
        {
            _colonistTraits.AddTrait(trait);
            _unit.AddTrait(trait);
            
            TraitsChange?.Invoke();
        }

        [Button(ButtonSizes.Medium)]
        public void RemoveTrait(Trait trait)
        {
            _colonistTraits.RemoveTrait(trait);
            _unit.RemoveTrait(trait);
            
            TraitsChange?.Invoke();
        }

        public void Select()
        {
            if (!_unit.Alive)
            {
                return;
            }

            _unitSelection.Select();
            _selectionIndicator.SetActive(true);
            _colonistMeshAgent.ShowLinePath();
            
            _unit.Select();
        }

        public void Deselect()
        {
            _unitSelection.Deselect();
            _selectionIndicator.SetActive(false);
            _colonistMeshAgent.HideLinePath();
            
            _unit.Deselect();
        }
    
        public void Stop()
        {
            _colonistBehavior.Stop();
        }
        
        public bool HasWeaponOf(WeaponType weaponType)
        {
            return _unit.HasWeaponOf(weaponType);
        }
        
        public void ChooseWeapon(WeaponType weaponType)
        {
            _unit.ChooseWeapon(weaponType);
        }

        public void OrderTo(Colonist targetColonist)
        {
            if (this == targetColonist)
            {
                return;
            }

            _colonistBehavior.OrderTo(targetColonist);
        }

        public void OrderTo(Unit unitTarget)
        {
            _colonistBehavior.OrderTo(unitTarget);
        }
        
        public void OrderTo(Resource resource)
        {
            _colonistBehavior.OrderTo(resource);
        }

        public void OrderToPosition(Vector3 position, float? angle)
        {
            _colonistBehavior.OrderToPosition(position, angle);
        }

        public void TryAddPositionToOrder(Vector3 position, float? angle)
        {
            _colonistBehavior.AddPositionToOrder(position, angle);
        }

        public void Die()
        {
            _unit.Die();
        }

        public void ToggleUnitFieldOfView()
        {
            _unit.ToggleUnitVisibilityFields();
        }

        public void ToggleResourceFieldOfView()
        {
            _colonistGatherer.ToggleResourceFieldOfView();
        }

        private void OnDying()
        {
            Stop();

            DeactivateComponents();

            Deselect();

            ColonistDying?.Invoke(this);
            Dying?.Invoke();

            _colonistAnimator.Die(DestroySelf);
        }

        private void Initialize()
        {
            _gender = EnumUtils.RandomValue<Gender>();

            if (_name == "")
            { 
                _name = _humanNames.GetRandomNameFor(_gender);
            }
                
            _humanAppearance.RandomizeAppearanceWith(_gender, HumanType.Colonist, _humanAppearanceRegistry);

            BindStatsToComponents();

            InitializeUnit();
        }

        private void InitializeUnit()
        {
            foreach (var trait in _colonistTraits.Traits)
            {
                _unit.AddTrait(trait);
            }

            _unit.Initialize();
        }

        private void BindStatsToComponents()
        {
            _colonistGatherer.BindStats(_colonistStats.ResourceDestructionSpeed, _colonistStats.ResourceExtractionEfficiency);
        }

        private void UnbindStatsFromComponents()
        {
            _colonistGatherer.UnbindStats(_colonistStats.ResourceDestructionSpeed, _colonistStats.ResourceExtractionEfficiency);
        }

        private void ActivateComponents()
        {
            _unitSelection.Activate();
            _colonistMeshAgent.Activate();
            _colonistBehavior.Activate();
        }

        private void DeactivateComponents()
        {
            _unit.HideUnitVisibilityFields();
            _colonistGatherer.HideResourceFieldOfView();

            _unitSelection.Deactivate();
            _unitAttacker.CoverUnitTarget();
            _unitAttacker.FinalizeAttackingInstantly();

            _colonistMeshAgent.Deactivate();
            _colonistBehavior.Deactivate();
        }

        private void DestroySelf()
        {
            Destroy(gameObject);
        }

        private void OnVitalityChange()
        {
            VitalityChange?.Invoke();
        }

        public class Factory : PlaceholderFactory<Colonist> { }
    }
}
