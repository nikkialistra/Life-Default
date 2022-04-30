using System;

namespace ResourceManagement
{
    public static class ResourceTypeExtensions
    {
        public static string GetStringForResources(this ResourceType resourceType)
        {
            return resourceType switch
            {
                ResourceType.Wood => "Trees",
                ResourceType.Stone => "Rocks",
                _ => throw new ArgumentOutOfRangeException(nameof(resourceType), resourceType, null)
            };
        }
        public static string GetStringForResourceChunks(this ResourceType resourceType)
        {
            return resourceType switch
            {
                ResourceType.Wood => "Logs",
                ResourceType.Stone => "Stones",
                _ => throw new ArgumentOutOfRangeException(nameof(resourceType), resourceType, null)
            };
        }

        public static string GetStringForMultiple(this ResourceType resourceType)
        {
            return resourceType switch
            {
                ResourceType.Wood => "woods",
                ResourceType.Stone => "stones",
                _ => throw new ArgumentOutOfRangeException(nameof(resourceType), resourceType, null)
            };
        }
    }
}
