using Buildings;
using Colonists.Colonist;
using Enemies.Enemy;
using Entities.Types;
using ResourceManagement;
using Sirenix.OdinInspector;
using UnityEngine;

namespace Entities
{
    public class Entity : MonoBehaviour
    {
        [SerializeField] private Transform _destinationPoint;
        [SerializeField] private GameObject _targetIndicator;

        [SerializeField] private EntityType _entityType;

        [ShowIf("_entityType", EntityType.Unit)]
        [ValidateInput("UnitEntityShouldHaveUnit", "Unit entity should have unit")]
        [SerializeField] private ColonistFacade _colonist;

        [ShowIf("_entityType", EntityType.Enemy)]
        [ValidateInput("EnemyEntityShouldHaveEnemy", "Enemy entity should have enemy")]
        [SerializeField] private EnemyFacade _enemy;

        [ShowIf("_entityType", EntityType.Building)]
        [ValidateInput("BuildingEntityShouldHaveBuilding", "Building entity should have resource")]
        [SerializeField] private Building _building;

        [ShowIf("_entityType", EntityType.Resource)]
        [ValidateInput("ResourceEntityShouldHaveResource", "Resource entity should have resource")]
        [SerializeField] private Resource _resource;

        public GameObject TargetIndicator => _targetIndicator;

        public EntityType EntityType => _entityType;
        public ColonistFacade Colonist => _colonist;
        public EnemyFacade Enemy => _enemy;
        public Building Building => _building;
        public Resource Resource => _resource;

        public Vector3 GetDestinationPoint()
        {
            return _destinationPoint.position;
        }

        private bool UnitEntityShouldHaveUnit()
        {
            if (_entityType == EntityType.Unit && _colonist == null)
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
