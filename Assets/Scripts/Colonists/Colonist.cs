using System;
using Common;
using Enemies;
using Entities;
using ResourceManagement;
using Sirenix.OdinInspector;
using Units;
using Units.Appearance;
using Units.Enums;
using UnityEngine;
using Zenject;
using static Units.Appearance.HumanAppearanceRegistry;

namespace Colonists
{
    [RequireComponent(typeof(Unit))]
    [RequireComponent(typeof(UnitSelection))]
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
            
            _animator = GetComponent<ColonistAnimator>();
            _meshAgent = GetComponent<ColonistMeshAgent>();
            _gatherer = GetComponent<ColonistGatherer>();
            _behavior = GetComponent<ColonistBehavior>();
        }

        public event Action Spawn;
        public event Action HealthChange;
        public event Action Die;
        public event Action<Colonist> ColonistDie;
        
        public event Action<string> NameChange;
        
        public event Action<Colonist> DestinationReach;

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
            _unit.HealthChange += OnHealthChange;
            _unit.Die += Dying;
            
            _meshAgent.DestinationReach += OnDestinationReach;
        }

        private void OnDisable()
        {
            _unit.HealthChange -= OnHealthChange;
            _unit.Die -= Dying;
            
            _meshAgent.DestinationReach -= OnDestinationReach;
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
            
            _unit.Select();

            _selectionIndicator.SetActive(true);
        }

        public void Deselect()
        {
            _unit.Deselect();

            _selectionIndicator.SetActive(false);
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

        public void OrderTo(Enemy enemy)
        {
            _behavior.OrderTo(enemy);
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

        public void ToggleUnitFieldOfView()
        {
            _unit.ToggleUnitVisibilityFields();
        }

        public void ToggleResourceFieldOfView()
        {
            _gatherer.ToggleResourceFieldOfView();
        }

        private void Dying()
        {
            Stop();

            DeactivateComponents();

            Deselect();

            Die?.Invoke();
            ColonistDie?.Invoke(this);

            _animator.Die(DestroySelf);
        }

        private void DeactivateComponents()
        {
            _meshAgent.Deactivate();
            _behavior.Deactivate();
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

        private void DestroySelf()
        {
            Destroy(gameObject);
        }

        private void OnDestinationReach()
        {
            DestinationReach?.Invoke(this);
        }

        private void OnHealthChange()
        {
            HealthChange?.Invoke();
        }

        public class Factory : PlaceholderFactory<Colonist> { }
    }
}
