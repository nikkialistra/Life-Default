using System;
using Colonists.Colonist.ColonistTypes;
using Entities;
using Entities.Ancillaries;
using Entities.Creature;
using Sirenix.OdinInspector;
using UnityEngine;
using Zenject;

namespace Colonists.Colonist
{
    [RequireComponent(typeof(EntityHealth))]
    [RequireComponent(typeof(ColonistMeshAgent))]
    [RequireComponent(typeof(EntityHovering))]
    [RequireComponent(typeof(ColonistAppearance))]
    [RequireComponent(typeof(ColonistBehavior))]
    public class ColonistFacade : MonoBehaviour
    {
        
        [Required]
        [SerializeField] private HealthBar _healthBar;
        [Required]
        [SerializeField] private ColonistAnimator _colonistAnimator;

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
        [SerializeField] private ColonistType _colonistType;
        [SerializeField] private string _name;
        
        [SerializeField] private ColonistIndicators _indicators;
        
        private bool _died;

        private EntityHealth _health;
        private EntityHovering _entityHovering;
        private ColonistAppearance _colonistAppearance;
        private ColonistMeshAgent _colonistMeshAgent;
        private ColonistBehavior _colonistBehavior;

        
        
        [Inject]
        public void Construct(Vector3 position)
        {
            transform.position = position;
        }

        private void Awake()
        {
            _health = GetComponent<EntityHealth>();
            _entityHovering = GetComponent<EntityHovering>();
            _colonistAppearance = GetComponent<ColonistAppearance>();
            _colonistMeshAgent = GetComponent<ColonistMeshAgent>();
            _colonistBehavior = GetComponent<ColonistBehavior>();
        }

        public event Action Spawn;
        public event Action HealthChange;
        public event Action Die;
        public event Action<ColonistFacade> ColonistDie;

        public event Action<ColonistFacade> DestinationReach;

        public ColonistType ColonistType => _colonistType;
        public string Name => name;
        
        public ColonistIndicators Indicators => _indicators;

        public int Health => _health.Health;
        
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

            _colonistMeshAgent.DestinationReach += OnDestinationReach;
        }

        private void OnDisable()
        {
            _health.HealthChange -= OnHealthChange;
            _health.Die -= Dying;

            _colonistMeshAgent.DestinationReach -= OnDestinationReach;
        }

        [Button(ButtonSizes.Large)]
        public void ChangeUnitType(ColonistType colonistType)
        {
            _colonistType = colonistType;
            _colonistAppearance.SwitchTo(colonistType);
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
            _colonistBehavior.Stop();
        }

        public bool TryOrderToEntity(Entity entity)
        {
            return _colonistBehavior.TryOrderToEntity(entity);
        }

        public bool TryOrderToPosition(Vector3 position, float? angle)
        {
            return _colonistBehavior.TryOrderToPosition(position, angle);
        }

        public bool TryAddPositionToOrder(Vector3 position, float? angle)
        {
            return _colonistBehavior.TryAddPositionToOrder(position, angle);
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
            ColonistDie?.Invoke(this);

            _colonistAnimator.Die(DestroySelf);
        }

        private void DeactivateComponents()
        {
            _entityHovering.Deactivate();
            _colonistMeshAgent.Deactivate();
            _colonistBehavior.Deactivate();
        }

        private void InitializeSelf()
        {
            _died = false;

            if (_name == "")
            {
                _name = ColonistNameGenerator.GetRandomName();
            }

            _health.Initialize();

            _healthBar.SetMaxHealth(_health.MaxHealth);
            _healthBar.SetHealth(_health.Health);

            _colonistAppearance.SwitchTo(_colonistType);
        }

        private void ActivateComponents()
        {
            _entityHovering.Activate();
            _colonistMeshAgent.Activate();
            _colonistBehavior.Activate();
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

        public class Factory : PlaceholderFactory<ColonistType, Vector3, ColonistFacade> { }
    }
}
