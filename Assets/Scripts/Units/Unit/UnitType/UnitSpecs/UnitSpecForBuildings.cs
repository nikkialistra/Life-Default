using System;
using Buildings;
using Common;
using Entities.Entity;

namespace Units.Unit.UnitType.UnitSpecs
{
    [Serializable]
    public class UnitSpecForBuildings
    {
        public UnitSpecForBuildingDictionary Buildings;
        
        [Serializable] public class UnitSpecForBuildingDictionary : SerializableDictionary<BuildingType, UnitSpecForBuilding> { }
        
        public bool CanInteractWithBuilding(Building building)
        {
            var buildingType = building.BuildingType;

            return Buildings.ContainsKey(buildingType);
        }
        
        public UnitSpecForBuilding GetUnitSpecForBuilding(Building building)
        {
            if (!CanInteractWithBuilding(building))
            {
                throw new InvalidOperationException("Unit spec cannot interact with this building");
            }

            return Buildings[building.BuildingType];
        }
    }
}
