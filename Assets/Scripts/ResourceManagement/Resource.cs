using System;
using Sirenix.OdinInspector;
using UnityEngine;

namespace ResourceManagement
{
    public class Resource : MonoBehaviour
    {
        [SerializeField] private ResourceType _resourceType;
        [MinValue(1)]
        [SerializeField] private int _quantity;
        
        [Space]
        [Required]
        [SerializeField] private Transform _holder;

        public ResourceType ResourceType => _resourceType;
        public int Quantity => _quantity;

        public ResourceOutput Extract(int value)
        {
            var extractedQuantity = ApplyExtraction(value);

            return new ResourceOutput(_resourceType, extractedQuantity);
        }
        
        [Button]
        public void Destroy()
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
