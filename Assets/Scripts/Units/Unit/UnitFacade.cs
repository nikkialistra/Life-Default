using System;
using Entities.Entity;
using Entities.Entity.Ancillaries;
using Sirenix.OdinInspector;
using Units.Unit.UnitTypes;
using UnityEngine;
using Zenject;

namespace Units.Unit
{
    [RequireComponent(typeof(EntityHealth))]
    [RequireComponent(typeof(UnitMeshAgent))]
    [RequireComponent(typeof(UnitAnimator))]
    [RequireComponent(typeof(UnitRenderer))]
    [RequireComponent(typeof(UnitAppearance))]
    [RequireComponent(typeof(UnitBehavior))]
    [RequireComponent(typeof(UnitClass))]
    [RequireComponent(typeof(UnitSaveLoadHandler))]
    public class UnitFacade : MonoBehaviour, IPoolable<UnitType, Vector3, IMemoryPool>, IDisposable
    {
        [Required]
        [SerializeField] private HealthBar _healthBar;
        [Required]
        [SerializeField] private GameObject _selectionIndicator;
        [Required]
        [SerializeField] private Transform _center;

        [Title("Properties")]
        [SerializeField] private UnitType _unitType;
        [SerializeField] private string _name;

        private bool _died;

        private EntityHealth _health;
        private UnitAnimator _unitAnimator;
        private UnitRenderer _unitRenderer;
        private UnitAppearance _unitAppearance;
        private UnitMeshAgent _unitMeshAgent;
        private UnitBehavior _unitBehavior;
        private UnitClass _unitClass;

        private IMemoryPool _pool;

        private void Awake()
        {
            _health = GetComponent<EntityHealth>();
            _unitAnimator = GetComponent<UnitAnimator>();
            _unitRenderer = GetComponent<UnitRenderer>();
            _unitAppearance = GetComponent<UnitAppearance>();
            _unitMeshAgent = GetComponent<UnitMeshAgent>();
            _unitBehavior = GetComponent<UnitBehavior>();
            _unitClass = GetComponent<UnitClass>();

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

        private void OnEnable()
        {
            _health.Die += Dispose;
            _health.HealthChange += OnHealthChange;

            _unitMeshAgent.DestinationReach += OnDestinationReach;
        }

        private void OnDisable()
        {
            _health.Die -= Dispose;
            _health.HealthChange -= OnHealthChange;

            _unitMeshAgent.DestinationReach -= OnDestinationReach;
        }

        [Button(ButtonSizes.Large)]
        public void ChangeUnitType(UnitType unitType)
        {
            _unitType = unitType;
            _unitAppearance.SwitchTo(unitType);

            _unitClass.ChangeUnitType(unitType);
            _unitBehavior.ChangeUnitClass(_unitClass);
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

            _unitRenderer.Select();

            _selectionIndicator.SetActive(true);
            _healthBar.Selected = true;
        }

        public void Deselect()
        {
            _unitRenderer.Deselect();

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

        public bool TryOrderToPosition(Vector3 position)
        {
            return _unitBehavior.TryOrderToPosition(position);
        }

        public void OnSpawned(UnitType unitType, Vector3 position, IMemoryPool pool)
        {
            _pool = pool;

            _unitType = unitType;
            transform.position = position;

            InitializeSelf();
            InitializeComponents();

            Spawn?.Invoke();
        }

        public void OnDespawned()
        {
            _pool = null;
        }

        public void Dispose()
        {
            _died = true;

            Stop();

            _unitMeshAgent.Deactivate();
            _unitBehavior.StopBehaviorTree();

            Deselect();

            Die?.Invoke();
            UnitDie?.Invoke(this);

            _unitAnimator.Die(DestroySelf);
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

        private void InitializeComponents()
        {
            _unitMeshAgent.Activate();

            _unitClass.ChangeUnitType(_unitType);

            _unitBehavior.Initialize();
            _unitBehavior.ChangeUnitClass(_unitClass);
            _unitBehavior.StartBehaviorTree();
        }

        private void DestroySelf()
        {
            _pool.Despawn(this);
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
