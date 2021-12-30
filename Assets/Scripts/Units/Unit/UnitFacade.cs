using System;
using Common;
using Entities;
using Sirenix.OdinInspector;
using UI.Game;
using Units.UnitTypes;
using UnityEngine;
using Zenject;

namespace Units.Unit
{
    [RequireComponent(typeof(EntityHealth))]
    [RequireComponent(typeof(UnitAnimator))]
    [RequireComponent(typeof(UnitModelElements))]
    [RequireComponent(typeof(UnitSaveLoadHandler))]
    public class UnitFacade : MonoBehaviour, IPoolable<UnitType, Vector3, IMemoryPool>, IDisposable
    {
        [Title("Properties")]
        [SerializeField] private UnitType _unitType;
        [SerializeField] private string _name;

        [Title("Indicators")]
        [Required] 
        [SerializeField] private HealthIndicatorView _healthIndicatorView;
        [Required]
        [SerializeField] private GameObject _selectionIndicator;

        public event Action Spawn;
        public event Action HealthChange;
        public event Action Die;

        public UnitSaveLoadHandler UnitSaveLoadHandler { get; private set; }

        public UnitType UnitType => _unitType;
        public string Name => _name;
        public int Health => _health.Health;
        public int MaxHealth => _health.MaxHealth;

        public bool Alive => !_died;

        private bool _died;

        private EntityHealth _health;
        private UnitAnimator _unitAnimator;
        private UnitModelElements _unitModelElements;

        private IMemoryPool _pool;

        private void Awake()
        {
            _health = GetComponent<EntityHealth>();
            _unitAnimator = GetComponent<UnitAnimator>();
            _unitModelElements = GetComponent<UnitModelElements>();
            
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
            _healthIndicatorView.Selected = true;
        }

        public void Deselect()
        {
            if (_died)
            {
                return;
            }
            
            _selectionIndicator.SetActive(false);
            _healthIndicatorView.Selected = false;
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

            _healthIndicatorView.SetMaxHealth(_health.MaxHealth);
            _healthIndicatorView.SetHealth(_health.Health);

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
            _healthIndicatorView.SetHealth(value);
            HealthChange?.Invoke();
        }

        public class Factory : PlaceholderFactory<UnitType, Vector3, UnitFacade>
        {
        }
    }
}