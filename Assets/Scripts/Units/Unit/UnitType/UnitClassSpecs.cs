using System;
using Entities.Entity;
using Sirenix.OdinInspector;
using Units.Unit.UnitType.UnitSpecs;
using UnityEngine;

namespace Units.Unit.UnitType
{
    [CreateAssetMenu(fileName = "UnitClassSpecs", menuName = "Data/Unit Class Specs")]
    public class UnitClassSpecs : ScriptableObject
    {
        [SerializeField] private UnitType _unitType;
        [SerializeField] private UnitTypeLevel _unitTypeLevel;

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

        public bool ContainsSpecFor(EntityType entityType)
        {
            return entityType switch
            {
                EntityType.Unit => _canInteractWithUnits,
                EntityType.Enemy => _canInteractWithEnemies,
                EntityType.Resource => _canInteractWithResources,
                EntityType.Building => _canInteractWithBuildings,
                _ => throw new ArgumentOutOfRangeException(nameof(entityType))
            };
        }

        public UnitSpecForUnits GetSpecForUnits()
        {
            if (!ContainsSpecFor(EntityType.Unit))
            {
                throw new InvalidOperationException("Unit class cannot interact with units");
            }

            return _unitSpecForUnits;
        }
        
        public UnitSpecForEnemies GetSpecForEnemies()
        {
            if (!ContainsSpecFor(EntityType.Enemy))
            {
                throw new InvalidOperationException("Unit class cannot interact with enemies");
            }

            return _unitSpecForEnemies;
        }
        
        public UnitSpecForResources GetSpecForResources()
        {
            if (!ContainsSpecFor(EntityType.Resource))
            {
                throw new InvalidOperationException("Unit class cannot interact with resources");
            }

            return _unitSpecForResources;
        }
        
        public UnitSpecForBuildings GetSpecForBuildings()
        {
            if (!ContainsSpecFor(EntityType.Building))
            {
                throw new InvalidOperationException("Unit class cannot interact with buildings");
            }

            return _unitSpecForBuildings;
        }
    }
}
