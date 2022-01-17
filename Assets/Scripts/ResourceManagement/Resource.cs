using System;
using Sirenix.OdinInspector;
using Units.Unit.UnitType;
using UnityEngine;

namespace ResourceManagement
{
    public class Resource : MonoBehaviour
    {
        [SerializeField] private ResourceType _resourceType;
        [SerializeField] private UnitType _canInteractWith;

        [MinValue(1)]
        [SerializeField] private int _health;
        [Range(0, 1)]
        [SerializeField] private float _resourcePerHp = 1;

        public int Health => _health;
        
        public bool CanInteractWith(UnitType unitType)
        {
            return unitType == _canInteractWith;
        }

        public ResourceOutput TakeDamage(int value)
        {
            var extractedQuantity = ApplyDamage(value);

            return new ResourceOutput(_resourceType, extractedQuantity);
        }

        private int ApplyDamage(int value)
        {
            if (_health <= 0)
            {
                throw new InvalidOperationException("Making damage cannot be applied to the destroyed resource");
            }
            
            int extractedQuantity;

            if (_health > value)
            {
                extractedQuantity = Mathf.RoundToInt(value * _resourcePerHp);
                _health -= value;
            }
            else
            {
                extractedQuantity = Mathf.RoundToInt(_health * _resourcePerHp);
                _health = 0;
            }

            return extractedQuantity;
        }
    }
}
