using System;
using Entities.Entity;
using Entities.Entity.Interfaces;
using Sirenix.OdinInspector;
using UnityEngine;

namespace ResourceManagement
{
    [RequireComponent(typeof(Entity))]
    public class Resource : MonoBehaviour, ICountable
    {
        [SerializeField] private ResourceType _resourceType;
        [MinValue(1)]
        [SerializeField] private int _quantity;

        [Space]
        [Required]
        [SerializeField] private Transform _holder;

        private int _acquiredCount = 0;

        private void Awake()
        {
            Entity = GetComponent<Entity>();
        }

        public Entity Entity { get; private set; }

        public ResourceType ResourceType => _resourceType;
        public bool Exausted => _quantity == 0;

        public ResourceOutput Extract(int value)
        {
            var extractedQuantity = ApplyExtraction(value);

            return new ResourceOutput(_resourceType, extractedQuantity);
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

            if (Exausted && _acquiredCount == 0)
            {
                Destroy();
            }
        }

        [Button]
        private void Destroy()
        {
            Destroy(_holder.gameObject);
        }

        private int ApplyExtraction(int value)
        {
            if (_quantity <= 0)
            {
                throw new InvalidOperationException("Making damage cannot be applied to the destroyed resource");
            }

            int extractedQuantity;

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
