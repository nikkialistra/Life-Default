using System;
using Buildings;
using Enemies.Enemy;
using ResourceManagement;
using Sirenix.OdinInspector;
using Units.Unit.UnitTypes.UnitSpecs;
using UnityEngine;

namespace Units.Unit.UnitTypes
{
    [CreateAssetMenu(fileName = "UnitClassSpecs", menuName = "Data/Unit Class Specs")]
    public class UnitTypeSpecs : ScriptableObject
    {
        [Title("Specs")]
        [Space]
        [SerializeField] private bool _canInteractWithUnits;
        [ShowIf("_canInteractWithUnits")]
        [SerializeField] private UnitSpecForUnits _unitSpecForUnits;
        [Space]
        [SerializeField] private bool _canInteractWithEnemies;
        [ShowIf("_canInteractWithEnemies")]
        [SerializeField] private UnitSpecForEnemies _unitSpecForEnemies;
        [Space]
        [SerializeField] private bool _canInteractWithResources;
        [ShowIf("_canInteractWithResources")]
        [SerializeField] private UnitSpecForResources _unitSpecForResources;
        [Space]
        [SerializeField] private bool _canInteractWithBuildings;
        [ShowIf("_canInteractWithBuildings")]
        [SerializeField] private UnitSpecForBuildings _unitSpecForBuildings;

        public bool CanInteractWith(UnitFacade unit)
        {
            return _canInteractWithUnits;
        }

        public bool CanInteractWith(EnemyFacade enemy)
        {
            return _canInteractWithEnemies;
        }

        public bool CanInteractWith(Building building)
        {
            return _canInteractWithBuildings && _unitSpecForBuildings.CanInteractWithBuilding(building);
        }

        public bool CanInteractWith(Resource resource)
        {
            return _canInteractWithResources && _unitSpecForResources.CanInteractWithResource(resource);
        }

        public UnitSpecForUnits GetSpecForUnits()
        {
            if (!_canInteractWithUnits)
            {
                throw new InvalidOperationException("Unit class cannot interact with units");
            }

            return _unitSpecForUnits;
        }

        public UnitSpecForEnemies GetSpecForEnemies()
        {
            if (!_canInteractWithEnemies)
            {
                throw new InvalidOperationException("Unit class cannot interact with enemies");
            }

            return _unitSpecForEnemies;
        }

        public UnitSpecForResource GetSpecForResource(Resource resource)
        {
            if (!CanInteractWith(resource))
            {
                throw new InvalidOperationException("Unit class cannot interact with resources");
            }

            return _unitSpecForResources.GetUnitSpecForResource(resource);
        }

        public UnitSpecForBuilding GetSpecForBuilding(Building building)
        {
            if (!CanInteractWith(building))
            {
                throw new InvalidOperationException("Unit class cannot interact with buildings");
            }

            return _unitSpecForBuildings.GetUnitSpecForBuilding(building);
        }
    }
}
