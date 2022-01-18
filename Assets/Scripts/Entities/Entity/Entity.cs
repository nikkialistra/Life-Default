using System;
using ResourceManagement;
using Sirenix.OdinInspector;
using UnityEngine;

namespace Entities.Entity
{
    public class Entity : MonoBehaviour
    {
        [SerializeField] private Transform _destinationPoint;
        [SerializeField] private GameObject _indicator;
        
        [SerializeField] private EntityType _entityType;
        [SerializeField] private ResourceType _resourceType;

        [MinValue(1)]
        [SerializeField] private int _health;
        [Range(0, 1)]
        [SerializeField] private float _resourcePerHp = 1;

        public int Health => _health;
        public EntityType EntityType => _entityType;

        public Vector3 GetDestinationPoint()
        {
            return _destinationPoint.position;
        }

        public void ShowIndicator()
        {
            _indicator.SetActive(true);
        }

        public void HideIndicator()
        {
            _indicator.SetActive(false);
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
