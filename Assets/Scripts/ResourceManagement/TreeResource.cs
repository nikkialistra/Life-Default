using System;
using Entities;
using Sirenix.OdinInspector;
using Units.Unit.UnitTypes;
using UnityEngine;

namespace ResourceManagement
{
    public class TreeResource : MonoBehaviour, IInteractable
    {
        [MinValue(1)]
        [SerializeField] private int _health;
        [MinValue(0)]
        [SerializeField] private int _resourcePerHp;

        public int Health => _health;
        
        public bool CanInteractWith(UnitType unitType)
        {
            return unitType == UnitType.Lumberjack;
        }

        public ResourceOutput TakeDamage(int value)
        {
            var extractedQuantity = ApplyDamage(value);

            return new ResourceOutput(ResourceType.Wood, extractedQuantity);
        }

        private int ApplyDamage(int value)
        {
            if (_health <= 0)
            {
                throw new InvalidOperationException("Making damage cannot be applied to the destroyed interactable");
            }
            
            int extractedQuantity;

            if (_health > value)
            {
                extractedQuantity = value * _resourcePerHp;
                _health -= value;
            }
            else
            {
                extractedQuantity = _health * _resourcePerHp;
                _health = 0;
            }

            return extractedQuantity;
        }
    }
}
