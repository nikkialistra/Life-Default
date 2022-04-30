using ResourceManagement;

namespace General.Questing
{
    public class QuestServices
    {
        private ResourceCounts _resourceCounts;

        public QuestServices(ResourceCounts resourceCounts)
        {
            _resourceCounts = resourceCounts;
        }

        public ResourceCounts ResourceCounts => _resourceCounts;
    }
}
