using System;
using Common;
using ResourceManagement;

namespace Units.Unit.UnitTypes.UnitSpecs
{
    [Serializable]
    public class UnitSpecForResources
    {
        public UnitSpecForResourceDictionary Resources;

        [Serializable]
        public class UnitSpecForResourceDictionary : SerializableDictionary<ResourceType, UnitSpecForResource> { }

        public bool CanInteractWithResource(Resource resource)
        {
            var resourceType = resource.ResourceType;

            return Resources.ContainsKey(resourceType);
        }

        public UnitSpecForResource GetUnitSpecForResource(Resource resource)
        {
            if (!CanInteractWithResource(resource))
            {
                throw new InvalidOperationException("Unit spec cannot interact with this resource");
            }

            return Resources[resource.ResourceType];
        }
    }
}
