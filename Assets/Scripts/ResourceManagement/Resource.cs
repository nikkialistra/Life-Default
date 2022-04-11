using System;
using Entities;
using Entities.Interfaces;
using Sirenix.OdinInspector;
using UnityEngine;

namespace ResourceManagement
{
    [RequireComponent(typeof(Entity))]
    [RequireComponent(typeof(Collider))]
    public class Resource : MonoBehaviour, ICountable
    {
        [SerializeField] private ResourceType _resourceType;
        [MinValue(1)]
        [SerializeField] private float _quantity;

        [Space]
        [Required]
        [SerializeField] private Transform _holder;

        private Collider _collider;

        private int _acquiredCount = 0;

        private void Awake()
        {
            _collider = GetComponent<Collider>();
            Entity = GetComponent<Entity>();
        }

        public Entity Entity { get; private set; }
        public ResourceType ResourceType => _resourceType;
        public bool Exhausted => _quantity == 0;

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
