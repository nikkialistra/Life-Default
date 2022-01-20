using System;
using System.Collections;
using Buildings;
using Enemies;
using Entities.Entity;
using Entities.Entity.Interfaces;
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

        private Coroutine _interactingCoroutine;

        private ICountable _acquired;

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
            if (!CanInteractWith(entity))
            {
                throw new InvalidOperationException("Unit class cannot interact with this entity");
            }

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

        public void StopInteraction()
        {
            if (_interactingCoroutine != null)
            {
                StopCoroutine(_interactingCoroutine);
                _unitAnimator.StopInteractWithResource();
            }

            ReleaseAcquired();
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

            _interactingCoroutine = StartCoroutine(InteractingWithResource(resource, unitSpecForResource));
        }

        private IEnumerator InteractingWithResource(Resource resource, UnitSpecForResource unitSpecForResource)
        {
            _unitAnimator.InteractWithResource();

            AddToAcquired(resource);

            while (!resource.Exausted)
            {
                yield return new WaitForSeconds(1f / unitSpecForResource.SpeedPerSecond);

                if (resource.Exausted)
                {
                    break;
                }

                var resourceOutput = resource.Extract(unitSpecForResource.Quantity);

                _resourceCounts.ChangeResourceTypeCount(resourceOutput.ResourceType, resourceOutput.Quantity);
            }

            ReleaseAcquired();

            _unitAnimator.StopInteractWithResource();
            _onInteractionFinish();
        }

        private void AddToAcquired(ICountable toAcquaire)
        {
            toAcquaire.Acquire();
            _acquired = toAcquaire;
        }

        private void ReleaseAcquired()
        {
            if (_acquired != null)
            {
                _acquired.Release();
                _acquired = null;
            }
        }
    }
}
