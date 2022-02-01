using Buildings;
using Enemies.Enemy;
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
        [SerializeField] private EnemyFacade _enemy;

        [ShowIf("_entityType", EntityType.Building)]
        [ValidateInput("BuildingEntityShouldHaveBuilding", "Building entity should have resource")]
        [SerializeField] private Building _building;

        [ShowIf("_entityType", EntityType.Resource)]
        [ValidateInput("ResourceEntityShouldHaveResource", "Resource entity should have resource")]
        [SerializeField] private Resource _resource;

        public GameObject TargetIndicator => _targetIndicator;

        public EntityType EntityType => _entityType;
        public UnitFacade Unit => _unit;
        public EnemyFacade Enemy => _enemy;
        public Building Building => _building;
        public Resource Resource => _resource;

        public Vector3 GetDestinationPoint()
        {
            return _destinationPoint.position;
        }
    }
}
