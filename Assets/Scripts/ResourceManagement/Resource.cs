using System;
using System.Collections;
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
    public class Resource : MonoBehaviour, ICountable, IHoverable
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
        
        [Title("Hover Settings")]
        [Required]
        [SerializeField] private MeshRenderer _renderer;
        [SerializeField] private int _hoverMaterialIndex;
        [SerializeField] private string _hoverPropertyName;
        
        [Space]
        [SerializeField] private Color _hoverColor;
        [SerializeField] private Color _clickColor;

        private int _emissiveColor;

        private WaitForSeconds _hoveringHideTime;
        private Coroutine _hideHoveringCoroutine;
        
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
            _hoveringHideTime = new WaitForSeconds(_timeToHideHover);
            _emissiveColor = Shader.PropertyToID(_hoverPropertyName);
        }

        public Entity Entity { get; private set; }
        
        public ResourceType ResourceType => _resourceType;
        
        public string Name => _name;
        public float Health => _health;
        public float Quantity => _quantity;
        
        public bool Exhausted => _quantity == 0;

        public void OnHover()
        {
            if (_hideHoveringCoroutine != null)
            {
                StopCoroutine(_hideHoveringCoroutine);
            }
            
            _infoPanelView.SetResource(this);
            _renderer.materials[_hoverMaterialIndex].SetColor(_emissiveColor, _hoverColor);

            _hideHoveringCoroutine = StartCoroutine(HideHoveringAfter());
        }

        private IEnumerator HideHoveringAfter()
        {
            yield return _hoveringHideTime;

            _infoPanelView.HideSelf();
            _renderer.materials[_hoverMaterialIndex].SetColor(_emissiveColor, Color.black);
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
