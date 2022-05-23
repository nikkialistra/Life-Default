using System;
using Common;
using ResourceManagement;
using Sirenix.OdinInspector;
using Units;
using Units.Ancillaries;
using Units.Appearance;
using Units.Enums;
using UnityEngine;
using Zenject;
using static Units.Appearance.HumanAppearanceRegistry;

namespace Colonists
{
    [RequireComponent(typeof(Unit))]
    [RequireComponent(typeof(UnitSelection))]
    [RequireComponent(typeof(UnitAttacker))]
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

        private ColonistAnimator _animator;
        private ColonistMeshAgent _meshAgent;
        private ColonistGatherer _gatherer;
        private ColonistBehavior _behavior;

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
            
            _animator = GetComponent<ColonistAnimator>();
            _meshAgent = GetComponent<ColonistMeshAgent>();
            _gatherer = GetComponent<ColonistGatherer>();
            _behavior = GetComponent<ColonistBehavior>();
        }

        public event Action Spawn;
        public event Action VitalityChange;
        public event Action Dying;
        public event Action<Colonist> ColonistDying;
        
        public event Action<string> NameChange;

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

        public void SetAt(Vector3 position)
        {
            transform.position = position;
        }

        [Button(ButtonSizes.Medium)]
        public void RandomizeAppearance()
        {
            _humanAppearance.RandomizeAppearanceWith(_gender, HumanType.Colonist, _humanAppearanceRegistry);
        }
        
        public void Select()
        {
            if (!_unit.Alive)
            {
                return;
            }

            _unitSelection.Select();
            _selectionIndicator.SetActive(true);
            _meshAgent.ShowLinePath();
            
            _unit.Select();
        }

        public void Deselect()
        {
            _unitSelection.Deselect();
            _selectionIndicator.SetActive(false);
            _meshAgent.HideLinePath();
            
            _unit.Deselect();
        }
    
        public void Stop()
        {
            _behavior.Stop();
        }

        public void OrderTo(Colonist targetColonist)
        {
            if (this == targetColonist)
            {
                return;
            }

            _behavior.OrderTo(targetColonist);
        }

        public void OrderTo(Unit unitTarget)
        {
            _behavior.OrderTo(unitTarget);
        }
        
        public void OrderTo(Resource resource)
        {
            _behavior.OrderTo(resource);
        }

        public void OrderToPosition(Vector3 position, float? angle)
        {
            _behavior.OrderToPosition(position, angle);
        }

        public void TryAddPositionToOrder(Vector3 position, float? angle)
        {
            _behavior.AddPositionToOrder(position, angle);
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
            _gatherer.ToggleResourceFieldOfView();
        }

        private void OnDying()
        {
            Stop();

            DeactivateComponents();

            Deselect();

            ColonistDying?.Invoke(this);
            Dying?.Invoke();

            _animator.Die(DestroySelf);
        }

        private void Initialize()
        {
            _gender = EnumUtils.RandomValue<Gender>();

            if (_name == "")
            { 
                _name = _humanNames.GetRandomNameFor(_gender);
            }
                
            _humanAppearance.RandomizeAppearanceWith(_gender, HumanType.Colonist, _humanAppearanceRegistry);

            _unit.Initialize();
        }

        private void ActivateComponents()
        {
            _unitSelection.Activate();
            _meshAgent.Activate();
            _behavior.Activate();
        }

        private void DeactivateComponents()
        {
            _unit.HideUnitVisibilityFields();
            _gatherer.HideResourceFieldOfView();

            _unitSelection.Deactivate();
            _unitAttacker.CoverUnitTarget();
            _unitAttacker.FinalizeAttackingInstantly();

            _meshAgent.Deactivate();
            _behavior.Deactivate();
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
