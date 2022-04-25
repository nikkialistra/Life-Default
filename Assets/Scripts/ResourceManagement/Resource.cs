using System;
using Entities;
using Entities.Interfaces;
using General.Map;
using ResourceManagement.Animations;
using Sirenix.OdinInspector;
using UI.Game.GameLook.Components;
using UnityEngine;
using Zenject;
using Random = UnityEngine.Random;

namespace ResourceManagement
{
    [RequireComponent(typeof(Entity))]
    [RequireComponent(typeof(Collider))]
    [RequireComponent(typeof(EntitySelection))]
    [RequireComponent(typeof(ResourceChunkScattering))]
    public class Resource : MonoBehaviour, ISelectableEntity
    {
        [SerializeField] private ResourceType _resourceType;
        [Space]
        [SerializeField] private string _name;
        [Space]
        [MinValue(1)]
        [SerializeField] private float _storedQuantity;
        [MinValue(1)]
        [SerializeField] private float _durability;
        [Space]
        [MinValue(1)]
        [SerializeField] private int _minExtractedQuantityForDrop;
        [MinValue(1)]
        [SerializeField] private int _maxExtractedQuantityForDrop;
        
        [Title("Configuration")]
        [Required]
        [SerializeField] private Transform _holder;
        [SerializeField] private int _clearDetailsRadius = 15;
        

        [Title("Animations")]
        [SerializeReference] private IAnimations _animations;

        private int _quantityToDrop;
        private float _preservedExtractedQuantity;

        private EntitySelection _entitySelection;

        private ResourceChunkScattering _resourceChunkScattering;

        private Collider _collider;

        private int _acquiredCount;

        private ResourceCounts _resourceCounts;

        private InfoPanelView _infoPanelView;
        
        private TerrainModification _terrainModification;

        [Inject]
        public void Construct(ResourceCounts resourceCounts, InfoPanelView infoPanelView, TerrainModification terrainModification)
        {
            _resourceCounts = resourceCounts;
            _infoPanelView = infoPanelView;
            
            _terrainModification = terrainModification;
        }

        private void Awake()
        {
            Entity = GetComponent<Entity>();
            _collider = GetComponent<Collider>();
            _entitySelection = GetComponent<EntitySelection>();
            _resourceChunkScattering = GetComponent<ResourceChunkScattering>();
        }

        public event Action QuantityChange;
        public event Action DurabilityChange;

        public Entity Entity { get; private set; }
        
        public ResourceType ResourceType => _resourceType;
        
        public string Name => _name;
        public int Quantity
        {
            get
            {
                var quantity = _storedQuantity + _preservedExtractedQuantity;
                return quantity >= 1f ? (int)Mathf.Floor(_storedQuantity + _preservedExtractedQuantity) : 0;
            }
        }
        public int Durability => (int)Mathf.Round(_durability);

        public bool Exhausted => _durability == 0;

        [Button(ButtonSizes.Medium)]
        public void ClearDetailsAround()
        {
            _terrainModification.ClearDetailsAt(transform.position, _clearDetailsRadius);
        }
        
        private void Start()
        {
            _quantityToDrop = CalculateNextQuantityToDrop();
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
            if (Exhausted)
            {
                return;
            }
            
            _entitySelection.Select();
            
            _infoPanelView.SetResource(this);
        }

        public void Deselect()
        {
            if (Exhausted)
            {
                return;
            }
            
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
            _preservedExtractedQuantity += ApplyDestruction(destructionValue) * extractionEfficiency;

            if (_preservedExtractedQuantity > _quantityToDrop)
            {
                DropPreservedQuantity();
            }
            else if (Exhausted && _preservedExtractedQuantity >= 1f)
            {
                DropRemainingQuantity();
            }
        }

        private void DropPreservedQuantity()
        {
            var sizeMultiplier = CalculateSizeMultiplier(_quantityToDrop, _maxExtractedQuantityForDrop);
            
            _resourceChunkScattering.Spawn(_resourceType, _quantityToDrop, sizeMultiplier);
            
            _preservedExtractedQuantity -= _quantityToDrop;
            _resourceCounts.ChangeResourceTypeCount(_resourceType, _quantityToDrop);

            _quantityToDrop = CalculateNextQuantityToDrop();
            
            QuantityChange?.Invoke();
        }

        private void DropRemainingQuantity()
        {
            var sizeMultiplier = CalculateSizeMultiplier(_preservedExtractedQuantity, _maxExtractedQuantityForDrop);
     
            _resourceChunkScattering.Spawn(_resourceType, (int)_preservedExtractedQuantity, sizeMultiplier);
            
            _resourceCounts.ChangeResourceTypeCount(_resourceType, (int)_preservedExtractedQuantity);
            
            QuantityChange?.Invoke();
        }

        private float CalculateSizeMultiplier(float quantity, int maxQuantity)
        {
            return Mathf.Sqrt(quantity / maxQuantity);
        }

        private int CalculateNextQuantityToDrop()
        {
            return Random.Range(_minExtractedQuantityForDrop, _maxExtractedQuantityForDrop + 1);
        }

        private void Destroy()
        {
            AstarPath.active.UpdateGraphs(_collider.bounds);

            Destroy(_holder.gameObject);
        }

        private float ApplyDestruction(float value)
        {
            if (_durability <= 0)
            {
                throw new InvalidOperationException("Making damage cannot be applied to the destroyed resource");
            }
            
            var quantityToDurabilityFraction = _storedQuantity / _durability;

            float extractedQuantity;

            if (_durability > value)
            {
                extractedQuantity = value * quantityToDurabilityFraction;
                _durability -= value;
            }
            else
            {
                extractedQuantity = _durability * quantityToDurabilityFraction;
                _durability = 0;
            }
            
            _storedQuantity -= extractedQuantity;
            
            DurabilityChange?.Invoke();

            return extractedQuantity;
        }
    }
}
