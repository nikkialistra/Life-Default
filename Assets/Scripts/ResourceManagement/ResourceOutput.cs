namespace ResourceManagement
{
    public class ResourceOutput
    {
        public readonly ResourceType ResourceType;
        public readonly int Quantity;

        public ResourceOutput(ResourceType resourceType, int quantity)
        {
            ResourceType = resourceType;
            Quantity = quantity;
        }
    }
}
