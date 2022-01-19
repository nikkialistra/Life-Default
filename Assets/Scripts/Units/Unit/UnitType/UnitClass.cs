using System;
using System.Collections;
using Buildings;
using Enemies;
using Entities.Entity;
using ResourceManagement;
using Units.Services;
using Units.Unit.UnitType.UnitSpecs;
using UnityEngine;
using Zenject;

namespace Units.Unit.UnitType
{
    [RequireComponent(typeof(UnitAnimator))]
    public class UnitClass : MonoBehaviour
    {
        private UnitClassSpecsRepository _unitClassSpecsRepository;
        private ResourceCounts _resourceCounts;
        private UnitClassSpecs UnitClassSpecs { get; set; }

        private Action _onInteractionFinish;
        
        private UnitAnimator _unitAnimator;

        [Inject]
        public void Construct(UnitClassSpecsRepository unitClassSpecsRepository, ResourceCounts resourceCounts)
        {
            _resourceCounts = resourceCounts;
            _unitClassSpecsRepository = unitClassSpecsRepository;
        }

        private void Awake()
        {
            _unitAnimator = GetComponent<UnitAnimator>();
        }

        public void ChangeUnitType(UnitType unitType)
        {
            UnitClassSpecs = _unitClassSpecsRepository.GetFor(unitType, UnitTypeLevel.FirstLevel);
        }

        public bool CanInteractWith(Entity entity)
        {
            return UnitClassSpecs.CanInteractWith(entity);
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
            var unitSpecForBuildings = UnitClassSpecs.GetSpecForBuilding(building);
        }

        private void InteractWithResource(Resource resource)
        {
            var unitSpecForResource = UnitClassSpecs.GetSpecForResource(resource);

            StartCoroutine(InteractingWithResource(resource, unitSpecForResource));
        }

        private IEnumerator InteractingWithResource(Resource resource, UnitSpecForResource unitSpecForResource)
        {
            _unitAnimator.InteractWithResource();

            while (resource.Quantity > 0)
            {
                yield return new WaitForSeconds(1f / unitSpecForResource.SpeedPerSecond);
                
                var resourceOutput = resource.Extract(unitSpecForResource.Quantity);
                
                _resourceCounts.ChangeResourceTypeCount(resourceOutput.ResourceType, resourceOutput.Quantity);
            }
            
            resource.Destroy();
            
            _unitAnimator.StopInteractWithResource();

            _onInteractionFinish();
        }
    }
}
