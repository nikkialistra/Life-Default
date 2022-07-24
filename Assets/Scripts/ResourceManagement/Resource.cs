using System;
using Entities;
using Map;
using ResourceManagement.Animations;
using Selecting;
using Sirenix.OdinInspector;
using UI.Game.GameLook.Components.Info;
using UnityEngine;
using Zenject;
using EntitySelection = Entities.EntitySelection;

namespace ResourceManagement
{
    [RequireComponent(typeof(Entity))]
    [RequireComponent(typeof(ResourceStorage))]
    [RequireComponent(typeof(EntitySelection))]
    [RequireComponent(typeof(ResourceChunkScattering))]
    [RequireComponent(typeof(Collider))]
    public class Resource : MonoBehaviour, ISelectable
    {
        public event Action<Resource> ResourceDestroying;
        public event Action Destroying;

        public event Action<int> QuantityChange;
        public event Action<int> DurabilityChange;

        public Entity Entity { get; private set; }

        public int Quantity => _storage.Quantity;

        public ResourceType ResourceType => _resourceType;

        public string Name => _name;

        public int Durability => _storage.Durability;

        public bool Exhausted => Durability == 0;

        [SerializeField] private ResourceType _resourceType;
        [Space]
        [SerializeField] private string _name;
        [Title("Configuration")]
        [Required]
        [SerializeField] private Transform _holder;
        [SerializeField] private int _clearDetailsRadius = 15;

        [Title("Animations")]
        [SerializeReference] private IAnimations _animations;

        private ResourceStorage _storage;

        private EntitySelection _entitySelection;

        private ResourceChunkScattering _resourceChunkScattering;

        private Collider _collider;

        private int _acquiredCount;

        private ResourceCounts _resourceCounts;

        private InfoPanelView _infoPanelView;

        private TerrainModification _terrainModification;

        [Inject]
        public void Construct(ResourceCounts resourceCounts, InfoPanelView infoPanelView,
            TerrainModification terrainModification)
        {
            _resourceCounts = resourceCounts;
            _infoPanelView = infoPanelView;

            _terrainModification = terrainModification;
        }

        private void Awake()
        {
            Entity = GetComponent<Entity>();

            _storage = GetComponent<ResourceStorage>();

            _collider = GetComponent<Collider>();
            _entitySelection = GetComponent<EntitySelection>();
            _resourceChunkScattering = GetComponent<ResourceChunkScattering>();
        }

        private void OnEnable()
        {
            _storage.ChunkDrop += SpawnChunk;

            _storage.QuantityChange += OnQuantityChange;
            _storage.DurabilityChange += OnDurabilityChange;
        }

        private void OnDisable()
        {
            _storage.ChunkDrop -= SpawnChunk;

            _storage.QuantityChange -= OnQuantityChange;
            _storage.DurabilityChange -= OnDurabilityChange;
        }

        [Button(ButtonSizes.Medium)]
        public void ClearDetailsAround()
        {
            _terrainModification.ClearDetailsAt(transform.position, _clearDetailsRadius);
        }

        public void Hover()
        {
            _entitySelection.Hover();
        }

        public void Flash()
        {
            _entitySelection.Flash();
        }

        public void Select()
        {
            if (Exhausted) return;

            _entitySelection.Select();

            _infoPanelView.SetResource(this);
        }

        public void Deselect()
        {
            if (Exhausted) return;

            _entitySelection.Deselect();

            _infoPanelView.UnsetResource(this);
        }

        public void StopDisplay()
        {
            _entitySelection.StopDisplay();
        }

        public void Hit(Vector3 position)
        {
            if (!Exhausted)
            {
                _animations.OnHit(position);
            }
            else
            {
                _infoPanelView.UnsetResource(this);
                StopDisplay();

                _animations.OnDestroy(position, Destroy);
            }
        }

        public void Extract(float destructionValue, float extractionEfficiency)
        {
            _storage.Extract(destructionValue, extractionEfficiency);
        }

        public void Destroy()
        {
            _storage.Reset();

            ResourceDestroying?.Invoke(this);
            Destroying?.Invoke();

            AstarPath.active.UpdateGraphs(_collider.bounds);

            Destroy(_holder.gameObject);
        }

        private void SpawnChunk(int quantity, float sizeMultiplier)
        {
            _resourceChunkScattering.Spawn(_resourceType, quantity, sizeMultiplier);

            _resourceCounts.ChangeResourceTypeCount(_resourceType, quantity);
        }

        private void OnQuantityChange(int quantity)
        {
            QuantityChange?.Invoke(quantity);
        }

        private void OnDurabilityChange(int durability)
        {
            DurabilityChange?.Invoke(Durability);
        }
    }
}
