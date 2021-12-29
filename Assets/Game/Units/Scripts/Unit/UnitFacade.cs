using System;
using Game.UI.Game;
using Game.Units.UnitTypes;
using Kernel.Entities;
using Kernel.Utils;
using Sirenix.OdinInspector;
using UnityEngine;
using Zenject;

namespace Game.Units.Unit
{
    [RequireComponent(typeof(EntityHealth))]
    [RequireComponent(typeof(UnitAnimator))]
    [RequireComponent(typeof(UnitSaveLoadHandler))]
    [RequireComponent(typeof(UnitTraveler))]
    public class UnitFacade : MonoBehaviour, IPoolable<Vector3, IMemoryPool>, IDisposable
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

        public UnitSaveLoadHandler UnitSaveLoadHandler { get; private set;  }

        public UnitType UnitType => _unitType;
        public string Name => _name;
        public int Health => _health.Health;
        public int MaxHealth => _health.MaxHealth;

        public bool Alive => !_died;

        private bool _died;

        private EntityHealth _health;
        private UnitAnimator _unitAnimator;
        private UnitTraveler _unitTraveler;

        private IMemoryPool _pool;

        private void Awake()
        {
            _health = GetComponent<EntityHealth>();
            _unitAnimator = GetComponent<UnitAnimator>();
            _unitTraveler = GetComponent<UnitTraveler>();
            
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
            Initialize();
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

        public void OnSpawned(Vector3 position, IMemoryPool pool)
        {
            _pool = pool;
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
                _name = NameGenerator.GetRandomName();
            }
            
            _health.Initialize();

            _healthIndicatorView.SetMaxHealth(_health.MaxHealth);
            _healthIndicatorView.SetHealth(_health.Health);

            SetUpTypeAppearance();
            
            Spawn?.Invoke();
        }

        private void SetUpTypeAppearance()
        {
            switch (UnitType)
            {
                case UnitType.Traveler:
                    _unitTraveler.Initialize();
                    break;
                case UnitType.Lumberjack:
                    break;
                case UnitType.Mason:
                    break;
                case UnitType.Melee:
                    break;
                case UnitType.Archer:
                    break;
                default:
                    throw new ArgumentOutOfRangeException();
            }
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

        public class Factory : PlaceholderFactory<Vector3, UnitFacade>
        {
        }
    }
}