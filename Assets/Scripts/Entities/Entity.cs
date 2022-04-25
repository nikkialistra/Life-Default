using Buildings;
using Colonists;
using Enemies;
using Entities.Types;
using ResourceManagement;
using Sirenix.OdinInspector;
using UnityEngine;

namespace Entities
{
    public class Entity : MonoBehaviour
    {
        [SerializeField] private EntityType _entityType;

        [ShowIf(nameof(_entityType), EntityType.Resource)]
        [ValidateInput(nameof(ResourceEntityShouldHaveResource), "Resource entity should have resource")]
        [SerializeField] private Resource _resource;
        
        [ShowIf(nameof(_entityType), EntityType.Building)]
        [ValidateInput(nameof(BuildingEntityShouldHaveBuilding), "Building entity should have resource")]
        [SerializeField] private Building _building;

        

        public EntityType EntityType => _entityType;
        
        public Resource Resource => _resource;
        public Building Building => _building;

        private bool ResourceEntityShouldHaveResource()
        {
            if (_entityType == EntityType.Resource && _resource == null)
            {
                return false;
            }

            return true;
        }

        private bool BuildingEntityShouldHaveBuilding()
        {
            if (_entityType == EntityType.Building && _building == null)
            {
                return false;
            }

            return true;
        }
    }
}
