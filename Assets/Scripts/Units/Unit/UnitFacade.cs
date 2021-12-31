using System;
using Entities;
using Sirenix.OdinInspector;
using UI.Game;
using UnitManagement.Targeting;
using Units.Unit.UnitTypes;
using UnityEngine;
using Zenject;

namespace Units.Unit
{
    [RequireComponent(typeof(EntityHealth))]
    [RequireComponent(typeof(UnitAnimator))]
    [RequireComponent(typeof(UnitModelElements))]
    [RequireComponent(typeof(UnitMeshAgent))]
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

        public event Action Spawn;
        public event Action HealthChange;
        public event Action Die;

        public ITargetable Targetable => _unitMeshAgent;

        public UnitSaveLoadHandler UnitSaveLoadHandler { get; private set; }

        public UnitType UnitType => _unitType;
        public string Name => _name;
        public int Health => _health.Health;
        public int MaxHealth => _health.MaxHealth;

        public bool Alive => !_died;
        public Vector3 Center => _center.position;

        private bool _died;

        private EntityHealth _health;
        private UnitAnimator _unitAnimator;
        private UnitModelElements _unitModelElements;
        private UnitMeshAgent _unitMeshAgent;

        private IMemoryPool _pool;

        private void Awake()
        {
            _health = GetComponent<EntityHealth>();
            _unitAnimator = GetComponent<UnitAnimator>();
            _unitModelElements = GetComponent<UnitModelElements>();
            _unitMeshAgent = GetComponent<UnitMeshAgent>();
            
            UnitSaveLoadHandler = GetComponent<UnitSaveLoadHandler>();
        }

        private void OnEnable()
        {
            _health.Die += Dispose;
            _health.HealthChange += OnHealthChange;
        }

        private void OnDisable()
        {
            _health.Die -= Dispose;
            _health.HealthChange -= OnHealthChange;
        }

        private void Start()
        {
            if (_pool == null)
            {
                Initialize();
            }
        }
        
        [Button(ButtonSizes.Large)]
        public void ChangeUnitType(UnitType unitType)
        {
            _unitType = unitType;
            _unitModelElements.SwitchTo(unitType);
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
            
            _selectionIndicator.SetActive(true);
            _healthBar.Selected = true;
        }

        public void Deselect()
        {
            if (_died)
            {
                return;
            }
            
            _selectionIndicator.SetActive(false);
            _healthBar.Selected = false;
        }

        public void OnSpawned(UnitType unitType, Vector3 position, IMemoryPool pool)
        {
            _pool = pool;
            
            _unitType = unitType;
            transform.position = position;

            Initialize();
        }

        public void OnDespawned()
        {
            _pool = null;
        }

        public void Dispose()
        {
            Die?.Invoke();
            
            Deselect();
            _died = true;
            _unitAnimator.Die(DestroySelf);
        }

        private void Initialize()
        {
            _died = false;
            
            if (_name == "")
            {
                _name = UnitNameGenerator.GetRandomName();
            }
            
            _health.Initialize();

            _healthBar.SetMaxHealth(_health.MaxHealth);
            _healthBar.SetHealth(_health.Health);

            _unitModelElements.SwitchTo(_unitType);
            
            Spawn?.Invoke();
        }

        private void DestroySelf()
        {
            if (_pool == null)
            {
                Destroy(gameObject);
            }
            else
            {
                _pool.Despawn(this);
            }
        }

        private void OnHealthChange(int value)
        {
            _healthBar.SetHealth(value);
            HealthChange?.Invoke();
        }

        public class Factory : PlaceholderFactory<UnitType, Vector3, UnitFacade>
        {
        }
    }
}