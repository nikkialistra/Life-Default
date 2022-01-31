using System;
using System.Collections;
using Buildings;
using Enemies.Enemy;
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

        private const string CannotInteract = "Unit class cannot interact with this entity type";

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

        public bool CanInteractWith(UnitFacade unit)
        {
            return UnitTypeSpecs.CanInteractWith(unit);
        }

        public bool CanInteractWith(EnemyFacade enemy)
        {
            return UnitTypeSpecs.CanInteractWith(enemy);
        }

        public bool CanInteractWith(Building building)
        {
            return UnitTypeSpecs.CanInteractWith(building);
        }

        public bool CanInteractWith(Resource resource)
        {
            return UnitTypeSpecs.CanInteractWith(resource);
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

        public void InteractWith(UnitFacade unit, Action onInteractionFinish)
        {
            if (!CanInteractWith(unit))
            {
                throw new InvalidOperationException(CannotInteract);
            }

            var unitSpecForUnits = UnitTypeSpecs.GetSpecForUnits();
        }

        public void InteractWith(EnemyFacade enemy, Action onInteractionFinish)
        {
            if (!CanInteractWith(enemy))
            {
                throw new InvalidOperationException(CannotInteract);
            }

            var unitSpecForEnemies = UnitTypeSpecs.GetSpecForEnemies();

            _interactingCoroutine =
                StartCoroutine(InteractingWithEnemy(enemy, unitSpecForEnemies, onInteractionFinish));
        }

        public void InteractWith(Building building, Action onInteractionFinish)
        {
            if (!CanInteractWith(building))
            {
                throw new InvalidOperationException(CannotInteract);
            }

            var unitSpecForBuildings = UnitTypeSpecs.GetSpecForBuilding(building);
        }

        public void InteractWith(Resource resource, Action onInteractionFinish)
        {
            if (!CanInteractWith(resource))
            {
                throw new InvalidOperationException(CannotInteract);
            }

            var unitSpecForResource = UnitTypeSpecs.GetSpecForResource(resource);

            _interactingCoroutine =
                StartCoroutine(InteractingWithResource(resource, unitSpecForResource, onInteractionFinish));
        }

        private IEnumerator InteractingWithEnemy(EnemyFacade enemy, UnitSpecForEnemies unitSpecForEnemies,
            Action onInteractionFinish)
        {
            _unitAnimator.InteractWithResource();

            while (CanInteract(enemy, unitSpecForEnemies))
            {
                yield return new WaitForSeconds(1f / unitSpecForEnemies.SpeedPerSecond);

                if (!CanInteract(enemy, unitSpecForEnemies))
                {
                    break;
                }

                enemy.TakeDamage(unitSpecForEnemies.Damage);
            }

            _unitAnimator.StopInteractWithResource();
            onInteractionFinish();
        }

        private IEnumerator InteractingWithResource(Resource resource, UnitSpecForResource unitSpecForResource,
            Action onInteractionFinish)
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
            onInteractionFinish();
        }

        private bool CanInteract(EnemyFacade enemy, UnitSpecForEnemies unitSpecForEnemies)
        {
            return enemy.Alive && Vector3.Distance(transform.position, enemy.transform.position) <
                unitSpecForEnemies.InteractionDistance;
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
