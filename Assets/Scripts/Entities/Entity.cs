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

        [ShowIf(nameof(_entityType), EntityType.Colonist)]
        [ValidateInput(nameof(UnitEntityShouldHaveUnit), "Colonist entity should have colonist")]
        [SerializeField] private ColonistFacade _colonist;

        [ShowIf(nameof(_entityType), EntityType.Enemy)]
        [ValidateInput(nameof(EnemyEntityShouldHaveEnemy), "Enemy entity should have enemy")]
        [SerializeField] private EnemyFacade _enemy;

        [ShowIf(nameof(_entityType), EntityType.Building)]
        [ValidateInput(nameof(BuildingEntityShouldHaveBuilding), "Building entity should have resource")]
        [SerializeField] private Building _building;

        [ShowIf(nameof(_entityType), EntityType.Resource)]
        [ValidateInput(nameof(ResourceEntityShouldHaveResource), "Resource entity should have resource")]
        [SerializeField] private Resource _resource;

        public EntityType EntityType => _entityType;
        public ColonistFacade Colonist => _colonist;
        public EnemyFacade Enemy => _enemy;
        public Building Building => _building;
        public Resource Resource => _resource;

        public Vector3 GetDestinationPoint()
        {
            return _destinationPoint.position;
        }

        public void Flash()
        {
            if (_entityType == EntityType.Resource)
            {
                _resource.Flash();
            }
        }

        private bool UnitEntityShouldHaveUnit()
        {
            if (_entityType == EntityType.Colonist && _colonist == null)
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
