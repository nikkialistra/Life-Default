namespace ResourceManagement
{
    public class ResourceOutput
    {
        public readonly ResourceType ResourceType;
        public readonly float Quantity;

        public ResourceOutput(ResourceType resourceType, float quantity)
        {
            ResourceType = resourceType;
            Quantity = quantity;
        }
    }
}
