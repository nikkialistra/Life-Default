using ResourceManagement;
using Sirenix.OdinInspector;
using UnityEngine;

namespace Entities.Entity
{
    public class Entity : MonoBehaviour
    {
        [SerializeField] private Transform _destinationPoint;
        [SerializeField] private GameObject _targetIndicator;
        
        [SerializeField] private EntityType _entityType;

        [ShowIf("_entityType", EntityType.Resource)]
        [Required]
        [SerializeField] private Resource _resource;
        
        public EntityType EntityType => _entityType;
        public Resource Resource => _resource;

        public Vector3 GetDestinationPoint()
        {
            return _destinationPoint.position;
        }

        public void ShowIndicator()
        {
            _targetIndicator.SetActive(true);
        }

        public void HideIndicator()
        {
            _targetIndicator.SetActive(false);
        }
    }
}
