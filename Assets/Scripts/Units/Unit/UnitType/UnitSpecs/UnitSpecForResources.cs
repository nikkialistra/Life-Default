using System;
using Common;
using ResourceManagement;

namespace Units.Unit.UnitType.UnitSpecs
{
    [Serializable]
    public class UnitSpecForResources
    {
        public UnitSpecForResourceDictionary Resources;
        
        [Serializable] public class UnitSpecForResourceDictionary : SerializableDictionary<ResourceType, UnitSpecForResource> { }
    }
}
