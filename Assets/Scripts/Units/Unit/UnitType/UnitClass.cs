using System;
using Entities.Entity;
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
                    InteractWithUnit(entity);
                    break;
                case EntityType.Enemy:
                    InteractWithEnemy(entity);
                    break;
                case EntityType.Resource:
                    InteractWithResource(entity);
                    break;
                case EntityType.Building:
                    InteractWithBuilding(entity);
                    break;
                default:
                    throw new ArgumentOutOfRangeException();
            }
        }

        private void InteractWithUnit(Entity entity)
        {
        
        }

        private void InteractWithEnemy(Entity entity)
        {
        
        }

        private void InteractWithResource(Entity entity)
        {
        
        }

        private void InteractWithBuilding(Entity entity)
        {
        
        }
    }
}
