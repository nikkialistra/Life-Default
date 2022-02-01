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
    public class UnitRole : MonoBehaviour
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

        public bool CanInteractWithUnits()
        {
            return UnitTypeSpecs.CanInteractWithUnits();
        }

        public bool CanInteractWithEnemies()
        {
            return UnitTypeSpecs.CanInteractWithEnemies();
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
            var unitSpecForUnits = UnitTypeSpecs.GetSpecForUnits();
        }

        public void InteractWith(EnemyFacade enemy, Action onInteractionFinish)
        {
            var unitSpecForEnemies = UnitTypeSpecs.GetSpecForEnemies();

            _interactingCoroutine =
                StartCoroutine(InteractingWithEnemy(enemy, unitSpecForEnemies, onInteractionFinish));
        }

        public void InteractWith(Building building, Action onInteractionFinish)
        {
            var unitSpecForBuildings = UnitTypeSpecs.GetSpecForBuilding(building);
        }

        public void InteractWith(Resource resource, Action onInteractionFinish)
        {
            var unitSpecForResource = UnitTypeSpecs.GetSpecForResource(resource);

            _interactingCoroutine =
                StartCoroutine(InteractingWithResource(resource, unitSpecForResource, onInteractionFinish));
        }

        public float GetInteractionDistanceWithEnemies()
        {
            return UnitTypeSpecs.GetSpecForEnemies().InteractionDistance;
        }

        public float GetAttackRangeDistanceWithEnemies()
        {
            return UnitTypeSpecs.GetSpecForEnemies().AttackRange;
        }

        public float GetInteractionDistanceWith(Resource resource)
        {
            return UnitTypeSpecs.GetSpecForResource(resource).InteractionDistance;
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

        private bool CanInteract(EnemyFacade enemy, UnitSpecForEnemies unitSpecForEnemies)
        {
            return enemy.Alive && Vector3.Distance(transform.position, enemy.transform.position) <
                unitSpecForEnemies.InteractionDistance;
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
