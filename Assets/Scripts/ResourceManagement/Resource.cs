using System;
using System.Collections;
using BehaviorDesigner.Runtime.Tasks.Unity.UnityLight;
using Entities;
using Entities.Interfaces;
using Sirenix.OdinInspector;
using UI.Game.GameLook.Components;
using UnityEngine;
using Zenject;

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
        [SerializeField] private float _health;
        [MinValue(1)]
        [SerializeField] private float _quantity;
        
        
        [Title("Configuration")]
        [Required]
        [SerializeField] private Transform _holder;
        [MinValue(0)]
        [SerializeField] private float _timeToHideHover = 0.05f;
        [SerializeField] private float _timeToDarkenSelection = 0.1f;
        
        [Title("Selection Settings")]
        [Required]
        [SerializeField] private MeshRenderer _renderer;
        [SerializeField] private int _materialIndex;
        [SerializeField] private string _propertyName;
        
        [Space]
        [SerializeField] private Color _hoverColor;
        [SerializeField] private Color _selectionColor;

        private int _emissiveColor;

        private Coroutine _hideHoveringCoroutine;

        private bool _selected;

        private Collider _collider;

        private int _acquiredCount = 0;

        private InfoPanelView _infoPanelView;

        [Inject]
        public void Construct(InfoPanelView infoPanelView)
        {
            _infoPanelView = infoPanelView;
        }

        private void Awake()
        {
            _collider = GetComponent<Collider>();
            Entity = GetComponent<Entity>();
        }

        private void Start()
        {
            _emissiveColor = Shader.PropertyToID(_propertyName);
        }

        public Entity Entity { get; private set; }
        
        public ResourceType ResourceType => _resourceType;
        
        public string Name => _name;
        public float Health => _health;
        public float Quantity => _quantity;
        
        public bool Exhausted => _quantity == 0;

        public void Hover()
        {
            if (_selected)
            {
                return;
            }
            
            if (_hideHoveringCoroutine != null)
            {
                StopCoroutine(_hideHoveringCoroutine);
            }
            
            SetColor(_hoverColor);

            _hideHoveringCoroutine = StartCoroutine(HideHoveringAfter());
        }

        public void Select()
        {
            _selected = true;
            _infoPanelView.SetResource(this);
            
            if (_hideHoveringCoroutine != null)
            {
                StopCoroutine(_hideHoveringCoroutine);
            }
            
            SetColor(_selectionColor);
        }

        public void Deselect()
        {
            _selected = false;
            SetColor(Color.black);
            
            _infoPanelView.UnsetEntityInfo();
        }

        private IEnumerator HideHoveringAfter()
        {
            yield return new WaitForSeconds(_timeToHideHover);
            
            SetColor(Color.black);
        }

        private void SetColor(Color color)
        {
            _renderer.materials[_materialIndex].SetColor(_emissiveColor, color);
        }

        public ResourceOutput Extract(float value, float extractionFraction)
        {
            var extractedQuantity = ApplyExtraction(value);

            return new ResourceOutput(_resourceType, extractedQuantity * extractionFraction);
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

            _acquiredCount--;

            if (Exhausted && _acquiredCount == 0)
            {
                Destroy();
            }
        }

        [Button]
        private void Destroy()
        {
            AstarPath.active.UpdateGraphs(_collider.bounds);

            Destroy(_holder.gameObject);
        }

        private float ApplyExtraction(float value)
        {
            if (_quantity <= 0)
            {
                throw new InvalidOperationException("Making damage cannot be applied to the destroyed resource");
            }

            float extractedQuantity;

            if (_quantity > value)
            {
                extractedQuantity = value;
                _quantity -= value;
            }
            else
            {
                extractedQuantity = _quantity;
                _quantity = 0;
            }

            return extractedQuantity;
        }
    }
}
