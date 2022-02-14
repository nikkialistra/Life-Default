using System;
using Entities;
using Entities.Ancillaries;
using Entities.Creature;
using Sirenix.OdinInspector;
using Units.Unit.UnitTypes;
using UnityEngine;
using Zenject;

namespace Units.Unit
{
    [RequireComponent(typeof(EntityHealth))]
    [RequireComponent(typeof(UnitMeshAgent))]
    [RequireComponent(typeof(EntityHovering))]
    [RequireComponent(typeof(UnitAppearance))]
    [RequireComponent(typeof(UnitBehavior))]
    [RequireComponent(typeof(UnitRole))]
    [RequireComponent(typeof(UnitSaveLoadHandler))]
    public class UnitFacade : MonoBehaviour
    {
        [Required]
        [SerializeField] private HealthBar _healthBar;
        [Required]
        [SerializeField] private UnitAnimator _unitAnimator;

        [Space]
        [Required]
        [SerializeField] private FieldOfView _enemyFieldOfView;
        [Required]
        [SerializeField] private FieldOfView _resourceFieldOfView;

        [Space]
        [Required]
        [SerializeField] private GameObject _selectionIndicator;
        [Required]
        [SerializeField] private Transform _center;

        [Title("Properties")]
        [SerializeField] private UnitType _unitType;
        [SerializeField] private string _name;

        private bool _died;

        private EntityHealth _health;
        private EntityHovering _entityHovering;
        private UnitAppearance _unitAppearance;
        private UnitMeshAgent _unitMeshAgent;
        private UnitBehavior _unitBehavior;
        private UnitRole _unitRole;

        [Inject]
        public void Construct(UnitType unitType, Vector3 position)
        {
            _unitType = unitType;
            transform.position = position;
        }

        private void Awake()
        {
            _health = GetComponent<EntityHealth>();
            _entityHovering = GetComponent<EntityHovering>();
            _unitAppearance = GetComponent<UnitAppearance>();
            _unitMeshAgent = GetComponent<UnitMeshAgent>();
            _unitBehavior = GetComponent<UnitBehavior>();
            _unitRole = GetComponent<UnitRole>();

            UnitSaveLoadHandler = GetComponent<UnitSaveLoadHandler>();
        }

        public event Action Spawn;
        public event Action HealthChange;
        public event Action Die;
        public event Action<UnitFacade> UnitDie;

        public event Action<UnitFacade> DestinationReach;

        public UnitSaveLoadHandler UnitSaveLoadHandler { get; private set; }

        public UnitType UnitType => _unitType;
        public string Name => _name;

        public int Health => _health.Health;
        public int MaxHealth => _health.MaxHealth;
        public bool Alive => !_died;

        public Vector3 Center => _center.position;

        private void Start()
        {
            InitializeSelf();
            ActivateComponents();

            Spawn?.Invoke();
        }

        private void OnEnable()
        {
            _health.HealthChange += OnHealthChange;
            _health.Die += Dying;

            _unitMeshAgent.DestinationReach += OnDestinationReach;
        }

        private void OnDisable()
        {
            _health.HealthChange -= OnHealthChange;
            _health.Die -= Dying;

            _unitMeshAgent.DestinationReach -= OnDestinationReach;
        }

        [Button(ButtonSizes.Large)]
        public void ChangeUnitType(UnitType unitType)
        {
            _unitType = unitType;
            _unitAppearance.SwitchTo(unitType);

            _unitRole.ChangeUnitType(unitType);
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

        public void Select()
        {
            if (_died)
            {
                return;
            }

            _entityHovering.Select();

            _selectionIndicator.SetActive(true);
            _healthBar.Selected = true;
        }

        public void Deselect()
        {
            _entityHovering.Deselect();

            _selectionIndicator.SetActive(false);
            _healthBar.Selected = false;
        }

        public void Stop()
        {
            _unitBehavior.Stop();
        }

        public bool TryOrderToEntity(Entity entity)
        {
            return _unitBehavior.TryOrderToEntity(entity);
        }

        public bool TryOrderToPosition(Vector3 position, float? angle)
        {
            return _unitBehavior.TryOrderToPosition(position, angle);
        }

        public bool TryAddPositionToOrder(Vector3 position, float? angle)
        {
            return _unitBehavior.TryAddPositionToOrder(position, angle);
        }

        public void ToggleEnemyFieldOfView()
        {
            _enemyFieldOfView.ToggleDebugShow();
        }

        public void ToggleResourceFieldOfView()
        {
            _resourceFieldOfView.ToggleDebugShow();
        }

        private void Dying()
        {
            _died = true;

            Stop();

            DeactivateComponents();

            Deselect();

            Die?.Invoke();
            UnitDie?.Invoke(this);

            _unitAnimator.Die(DestroySelf);
        }

        private void DeactivateComponents()
        {
            _entityHovering.Deactivate();
            _unitMeshAgent.Deactivate();
            _unitBehavior.Deactivate();
        }

        private void InitializeSelf()
        {
            _died = false;

            if (_name == "")
            {
                _name = UnitNameGenerator.GetRandomName();
            }

            _health.Initialize();

            _healthBar.SetMaxHealth(_health.MaxHealth);
            _healthBar.SetHealth(_health.Health);

            _unitAppearance.SwitchTo(_unitType);
        }

        private void ActivateComponents()
        {
            _entityHovering.Activate();
            _unitMeshAgent.Activate();
            _unitBehavior.Activate();
            _unitRole.ChangeUnitType(_unitType);
        }

        private void DestroySelf()
        {
            Destroy(gameObject);
        }

        private void OnDestinationReach()
        {
            DestinationReach?.Invoke(this);
        }

        private void OnHealthChange(int value)
        {
            _healthBar.SetHealth(value);
            HealthChange?.Invoke();
        }

        public class Factory : PlaceholderFactory<UnitType, Vector3, UnitFacade> { }
    }
}
