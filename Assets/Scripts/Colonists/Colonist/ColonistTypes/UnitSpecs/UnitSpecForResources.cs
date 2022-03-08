using System;
using Common;
using ResourceManagement;

namespace Colonists.Colonist.ColonistTypes.UnitSpecs
{
    [Serializable]
    public class UnitSpecForResources
    {
        public UnitSpecForResourceDictionary Resources;
        
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
        
        [Serializable]
        public class UnitSpecForResourceDictionary : SerializableDictionary<ResourceType, UnitSpecForResource> { }
    }
}
