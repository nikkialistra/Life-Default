using System;
using System.Collections;
using Buildings;
using Enemies.Enemy;
using Entities.Entity;
using Entities.Entity.Interfaces;
using ResourceManagement;
using Units.Services;
using Units.Unit.UnitTypes.UnitSpecs;
using UnityEngine;
using Zenject;

namespace Units.Unit.UnitTypes
{
    [RequireComponent(typeof(UnitAnimator))]
    public class UnitClass : MonoBehaviour
    {
        private UnitTypeSpecsRepository _unitTypeSpecsRepository;
        private ResourceCounts _resourceCounts;
        private UnitTypeSpecs UnitTypeSpecs { get; set; }

        private Action _onInteractionFinish;

        private UnitAnimator _unitAnimator;

        private Coroutine _interactingCoroutine;

        private ICountable _acquired;

        [Inject]
        public void Construct(UnitTypeSpecsRepository unitTypeSpecsRepository, ResourceCounts resourceCounts)
        {
            _resourceCounts = resourceCounts;
            _unitTypeSpecsRepository = unitTypeSpecsRepository;
        }

        private void Awake()
        {
            _unitAnimator = GetComponent<UnitAnimator>();
        }

        public void ChangeUnitType(UnitType unitType)
        {
            UnitTypeSpecs = _unitTypeSpecsRepository.GetFor(unitType, UnitTypeLevel.FirstLevel);
        }

        public bool CanInteractWith(Entity entity)
        {
            return UnitTypeSpecs.CanInteractWith(entity);
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
            var unitSpecForUnits = UnitTypeSpecs.GetSpecForUnits();
        }

        private void InteractWithEnemy(EnemyFacade enemy)
        {
            var unitSpecForEnemies = UnitTypeSpecs.GetSpecForEnemies();

            _interactingCoroutine = StartCoroutine(InteractingWithEnemy(enemy, unitSpecForEnemies));
        }

        private IEnumerator InteractingWithEnemy(EnemyFacade enemy, UnitSpecForEnemies unitSpecForEnemies)
        {
            _unitAnimator.InteractWithResource();

            while (enemy.Alive)
            {
                yield return new WaitForSeconds(1f / unitSpecForEnemies.SpeedPerSecond);

                if (!enemy.Alive)
                {
                    break;
                }

                enemy.TakeDamage(unitSpecForEnemies.Damage);
            }

            _unitAnimator.StopInteractWithResource();
            _onInteractionFinish();
        }

        private void InteractWithBuilding(Building building)
        {
            var unitSpecForBuildings = UnitTypeSpecs.GetSpecForBuilding(building);
        }

        private void InteractWithResource(Resource resource)
        {
            var unitSpecForResource = UnitTypeSpecs.GetSpecForResource(resource);

            _interactingCoroutine = StartCoroutine(InteractingWithResource(resource, unitSpecForResource));
        }

        private IEnumerator InteractingWithResource(Resource resource, UnitSpecForResource unitSpecForResource)
        {
            _unitAnimator.InteractWithResource();

            AddToAcquired(resource);

            while (!resource.Exhausted)
            {
                yield return new WaitForSeconds(1f / unitSpecForResource.SpeedPerSecond);

                if (resource.Exhausted)
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
