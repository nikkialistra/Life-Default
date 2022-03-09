using System;
using Entities;
using Entities.Ancillaries;
using Entities.Creature;
using Sirenix.OdinInspector;
using UnityEngine;
using Zenject;

namespace Colonists.Colonist
{
    [RequireComponent(typeof(EntityHealth))]
    [RequireComponent(typeof(ColonistIndicators))]
    [RequireComponent(typeof(ColonistMeshAgent))]
    [RequireComponent(typeof(EntityHovering))]
    [RequireComponent(typeof(ColonistBehavior))]
    public class ColonistFacade : MonoBehaviour
    {
        [Required]
        [SerializeField] private HealthBars _healthBars;
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
        [SerializeField] private string _name;
        
        [Required]
        [SerializeField] private ColonistIndicators _colonistIndicators;
        
        private bool _died;

        private EntityHovering _entityHovering;
        private ColonistMeshAgent _colonistMeshAgent;
        private ColonistBehavior _colonistBehavior;

        [Inject]
        public void Construct(Vector3 position)
        {
            transform.position = position;
        }

        private void Awake()
        {
            Health = GetComponent<EntityHealth>();
            Indicators = GetComponent<ColonistIndicators>();
            _entityHovering = GetComponent<EntityHovering>();
            _colonistMeshAgent = GetComponent<ColonistMeshAgent>();
            _colonistBehavior = GetComponent<ColonistBehavior>();
        }

        public event Action Spawn;
        public event Action HealthChange;
        public event Action Die;
        public event Action<ColonistFacade> ColonistDie;

        public event Action<ColonistFacade> DestinationReach;
        
        public string Name => _name;
        
        public EntityHealth Health { get; private set; }
        public ColonistIndicators Indicators { get; private set; }

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
            Health.HealthChange += OnHealthChange;
            Health.Die += Dying;

            _colonistMeshAgent.DestinationReach += OnDestinationReach;
        }

        private void OnDisable()
        {
            Health.HealthChange -= OnHealthChange;
            Health.Die -= Dying;

            _colonistMeshAgent.DestinationReach -= OnDestinationReach;
        }

        [Button(ButtonSizes.Large)]
        public void TakeDamage(float value)
        {
            if (_died)
            {
                return;
            }

            Health.TakeDamage(value);
        }

        public void Select()
        {
            if (_died)
            {
                return;
            }

            _entityHovering.Select();

            _selectionIndicator.SetActive(true);
            _healthBars.Selected = true;
        }

        public void Deselect()
        {
            _entityHovering.Deselect();

            _selectionIndicator.SetActive(false);
            _healthBars.Selected = false;
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

            Health.Initialize();
            
            _healthBars.SetVitality(Health.Vitality);
            _healthBars.SetBlood(Health.Blood);
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

        private void OnHealthChange(float vitality, float blood)
        {
            _healthBars.SetVitality(vitality);
            _healthBars.SetBlood(blood);
            HealthChange?.Invoke();
        }

        public class Factory : PlaceholderFactory<Vector3, ColonistFacade> { }
    }
}
