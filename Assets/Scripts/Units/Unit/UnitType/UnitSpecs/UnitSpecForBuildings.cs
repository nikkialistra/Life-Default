using System;
using Common;
using Entities;
using Entities.Entity;

namespace Units.Unit.UnitType.UnitSpecs
{
    [Serializable]
    public class UnitSpecForBuildings
    {
        public UnitSpecForBuildingDictionary Buildings;
        
        [Serializable] public class UnitSpecForBuildingDictionary : SerializableDictionary<BuildingType, UnitSpecForBuilding> { }
    }
}
