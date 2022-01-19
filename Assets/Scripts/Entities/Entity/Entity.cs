using Buildings;
using Enemies;
using ResourceManagement;
using Sirenix.OdinInspector;
using Units.Unit;
using UnityEngine;

namespace Entities.Entity
{
    public class Entity : MonoBehaviour
    {
        [SerializeField] private Transform _destinationPoint;
        [SerializeField] private GameObject _targetIndicator;
        
        [SerializeField] private EntityType _entityType;

        [ShowIf("_entityType", EntityType.Unit)]
        [ValidateInput("UnitEntityShouldHaveUnit", "Unit entity should have unit")]
        [SerializeField] private UnitFacade _unit;
        
        [ShowIf("_entityType", EntityType.Enemy)]
        [ValidateInput("EnemyEntityShouldHaveEnemy", "Enemy entity should have enemy")]
        [SerializeField] private Enemy _enemy;
        
        [ShowIf("_entityType", EntityType.Building)]
        [ValidateInput("BuildingEntityShouldHaveBuilding", "Building entity should have resource")]
        [SerializeField] private Building _building;
        
        [ShowIf("_entityType", EntityType.Resource)]
        [ValidateInput("ResourceEntityShouldHaveResource", "Resource entity should have resource")]
        [SerializeField] private Resource _resource;

        public EntityType EntityType => _entityType;
        public UnitFacade Unit => _unit;
        public Enemy Enemy => _enemy;
        public Building Building => _building;
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

        private bool UnitEntityShouldHaveUnit()
        {
            if (_entityType == EntityType.Unit && _unit == null)
            {
                return false;
            }

            return true;
        }

        private bool EnemyEntityShouldHaveEnemy()
        {
            if (_entityType == EntityType.Enemy && _enemy == null)
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

        private bool ResourceEntityShouldHaveResource()
        {
            if (_entityType == EntityType.Resource && _resource == null)
            {
                return false;
            }

            return true;
        }
    }
}
