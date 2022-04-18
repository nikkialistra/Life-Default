using System;
using System.Collections;
using Entities;
using Entities.Animations;
using Entities.Interfaces;
using Sirenix.OdinInspector;
using UI.Game.GameLook.Components;
using UnityEngine;
using Zenject;
using Random = UnityEngine.Random;

namespace ResourceManagement
{
    [RequireComponent(typeof(Entity))]
    [RequireComponent(typeof(Collider))]
    public class Resource : MonoBehaviour, ICountable, ISelectable
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
        [MinValue(0)]
        [SerializeField] private float _timeToHideHover = 0.05f;
        [SerializeField] private float _timeToHideSelection = 0.12f;

        [Title("Selection Settings")]
        [Required]
        [SerializeField] private MeshRenderer _renderer;
        [SerializeField] private int _materialIndex;
        [SerializeField] private string _propertyName;
        
        [Space]
        [SerializeField] private Color _hoverColor;
        [SerializeField] private Color _selectionColor;
        
        [Title("Animations")]
        [SerializeReference] private IAnimations _animations;

        private int _requiredQuantityToDrop;
        private float _preservedExtractedQuantity;

        private int _emissiveColor;

        private Coroutine _hoveringCoroutine;
        private Coroutine _hideSelectionCoroutine;

        private bool _hovered;
        private bool _selected;

        private Collider _collider;

        private int _acquiredCount;

        private ResourceCounts _resourceCounts;

        private InfoPanelView _infoPanelView;

        [Inject]
        public void Construct(ResourceCounts resourceCounts, InfoPanelView infoPanelView)
        {
            _resourceCounts = resourceCounts;
            _infoPanelView = infoPanelView;
        }

        private void Awake()
        {
            _collider = GetComponent<Collider>();
            Entity = GetComponent<Entity>();
        }

        public event Action QuantityChange;
        public event Action DurabilityChange;

        public Entity Entity { get; private set; }
        
        public ResourceType ResourceType => _resourceType;
        
        public string Name => _name;
        public float Quantity
        {
            get
            {
                var quantity = _storedQuantity + _preservedExtractedQuantity;
                return quantity >= 1f ? Mathf.Floor(_storedQuantity + _preservedExtractedQuantity) : 0;
            }
        }
        public float Durability => _durability;

        public bool Exhausted => _durability == 0;
        
        private void Start()
        {
            _emissiveColor = Shader.PropertyToID(_propertyName);
            _requiredQuantityToDrop = CalculateNextRequiredQuantityToDrop();

            if (_animations != null)
            {
                _animations.Initialize();
            }
        }

        public void Hover()
        {
            if (_hovered || _selected)
            {
                return;
            }

            _hovered = true;
            
            _hoveringCoroutine ??= StartCoroutine(Hovering());
        }

        private IEnumerator Hovering()
        {
            SetColor(_hoverColor);

            while (true)
            {
                _hovered = false;

                yield return new WaitForSecondsRealtime(_timeToHideHover);

                if (!_hovered)
                {
                    SetColor(Color.black);
                    break;
                }
            }
            
            _hoveringCoroutine = null;
        }

        public void Select()
        {
            if (Exhausted)
            {
                return;
            }
            
            StopDisplayChangingCoroutines();

            _hovered = false;
            _selected = true;
            _infoPanelView.SetResource(this);

            SetColor(_selectionColor);
        }

        public void Flash()
        {
            StopDisplayChangingCoroutines();
            
            SetColor(_selectionColor);

            _hovered = false;
            _selected = true;

            _hideSelectionCoroutine = StartCoroutine(HideSelectionAfter());
        }

        private void StopDisplayChangingCoroutines()
        {
            if (_hoveringCoroutine != null)
            {
                StopCoroutine(_hoveringCoroutine);
                _hoveringCoroutine = null;
            }

            if (_hideSelectionCoroutine != null)
            {
                StopCoroutine(_hideSelectionCoroutine);
                _hideSelectionCoroutine = null;
            }
        }

        public void Deselect()
        {
            if (Exhausted)
            {
                return;
            }
            
            _selected = false;
            SetColor(Color.black);
            
            _infoPanelView.UnsetResource(this);
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
                StopDisplayChangingCoroutines();
                
                _animations.OnDestroy(position, Destroy);
            }
        }
        
        public void Acquire()
        {
            _acquiredCount++;
        }

        public void Release()
        {
            if (_acquiredCount == 0)
            {
                throw new InvalidOperationException("Cannot release resource which not acquired");
            }

            Deselect();

            _acquiredCount--;
        }

        private IEnumerator HideSelectionAfter()
        {
            yield return new WaitForSecondsRealtime(_timeToHideSelection);
            
            SetColor(Color.black);
            _selected = false;
        }

        private void SetColor(Color color)
        {
            _renderer.materials[_materialIndex].SetColor(_emissiveColor, color);
        }

        public void Extract(float destructionValue, float extractionEfficiency)
        {
            _preservedExtractedQuantity += ApplyDestruction(destructionValue) * extractionEfficiency;

            if (_preservedExtractedQuantity > _requiredQuantityToDrop)
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
            _preservedExtractedQuantity -= _requiredQuantityToDrop;
            _resourceCounts.ChangeResourceTypeCount(_resourceType, _requiredQuantityToDrop);

            _requiredQuantityToDrop = CalculateNextRequiredQuantityToDrop();
            
            QuantityChange?.Invoke();
        }

        private void DropRemainingQuantity()
        {
            _resourceCounts.ChangeResourceTypeCount(_resourceType, (int)_preservedExtractedQuantity);
            
            QuantityChange?.Invoke();
        }

        private int CalculateNextRequiredQuantityToDrop()
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
