using System;
using System.Collections;
using Game.UI.Game;
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
        
        public event Action<int> HealthChange;
        public event Action Die;

        public UnitSaveLoadHandler UnitSaveLoadHandler { get; private set;  }

        public UnitType UnitType => _unitType;
        public string Name => _name;
        public int Health => _health.Health;
        public int MaxHealth => _health.MaxHealth;

        private bool _died;
        
        private UnitAnimator _unitAnimator;
        private EntityHealth _health;

        private IMemoryPool _pool;

        private void Awake()
        {
            _health = GetComponent<EntityHealth>();
            _unitAnimator = GetComponent<UnitAnimator>();
            UnitSaveLoadHandler = GetComponent<UnitSaveLoadHandler>();
        }

        private void OnEnable()
        {
            _health.Die += Dispose;
            _health.HealthChange += OnHealthChange;
            _unitAnimator.DeathFinish += DestroySelf;
        }

        private void OnDisable()
        {
            _health.Die -= Dispose;
            _health.HealthChange -= OnHealthChange;
            _unitAnimator.DeathFinish -= DestroySelf;
        }

        private void Start()
        {
            if (_name == "")
            {
                _name = NameGenerator.GetRandomName();
            }

            _healthIndicatorView.SetMaxHealth(_health.MaxHealth);
            _healthIndicatorView.SetHealth(_health.Health);
            
            StartCoroutine(TakingDamage());
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

        private IEnumerator TakingDamage()
        {
            while (true)
            {
                TakeDamage(15);
                yield return new WaitForSeconds(1f);
            }
        }

        public void Select()
        {
            if (_died)
            {
                return;
            }
            
            _selectionIndicator.SetActive(true);
            _healthIndicatorView.Show();
        }

        public void Deselect()
        {
            _selectionIndicator.SetActive(false);
            _healthIndicatorView.Hide();
        }

        public void OnSpawned(Vector3 position, IMemoryPool pool)
        {
            _pool = pool;
            transform.position = position;
        }

        public void OnDespawned()
        {
            _pool = null;
        }

        public void Dispose()
        {
            Deselect();
            Die?.Invoke();
            _died = true;
        }

        private void DestroySelf()
        {
            _unitAnimator.DeathFinish -= DestroySelf;

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
            HealthChange?.Invoke(value);
        }

        public class Factory : PlaceholderFactory<Vector3, UnitFacade>
        {
        }
    }
}