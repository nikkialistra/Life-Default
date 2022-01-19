using System;
using Buildings;
using Enemies;
using Entities.Entity;
using ResourceManagement;
using Units.Services;
using UnityEngine;
using Zenject;

namespace Units.Unit.UnitType
{
    public class UnitClass : MonoBehaviour
    {
        private UnitClassSpecsRepository _unitClassSpecsRepository;
        private UnitClassSpecs UnitClassSpecs { get; set; }
        
        private Action _onInteractionFinish;

        [Inject]
        public void Construct(UnitClassSpecsRepository unitClassSpecsRepository)
        {
            _unitClassSpecsRepository = unitClassSpecsRepository;
        }

        public void ChangeUnitType(UnitType unitType)
        {
            UnitClassSpecs = _unitClassSpecsRepository.GetFor(unitType, UnitTypeLevel.FirstLevel);
        }

        public bool CanInteractWith(Entity entity)
        {
            var entityType = entity.EntityType;

            return UnitClassSpecs.ContainsSpecFor(entityType);
        }

        public void InteractWith(Entity entity, Action onInteractionFinish)
        {
            _onInteractionFinish = onInteractionFinish;
            
            switch (entity.EntityType)
            {
                case EntityType.Unit:
                    InteractWithUnit(entity.Unit);
                    break;
                case EntityType.Enemy:
                    InteractWithEnemy(entity.Enemy);
                    break;
                case EntityType.Building:
                    InteractWithBuilding(entity.Building);
                    break;
                case EntityType.Resource:
                    InteractWithResource(entity.Resource);
                    break;
                default:
                    throw new ArgumentOutOfRangeException();
            }
        }

        private void InteractWithUnit(UnitFacade unit)
        {
            var unitSpecForUnits = UnitClassSpecs.GetSpecForUnits();
        }

        private void InteractWithEnemy(Enemy enemy)
        {
            var unitSpecForEnemies = UnitClassSpecs.GetSpecForEnemies();
        }

        private void InteractWithBuilding(Building building)
        {
            var unitSpecForBuildings = UnitClassSpecs.GetSpecForBuildings();
        }

        private void InteractWithResource(Resource resource)
        {
            var unitSpecForResources = UnitClassSpecs.GetSpecForResources();
        }
    }
}
