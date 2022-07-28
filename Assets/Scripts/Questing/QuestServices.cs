using Aborigines.Services;
using ResourceManagement;

namespace Questing
{
    public class QuestServices
    {
        public QuestServices(ResourceCounts resourceCounts, AborigineRepository aborigineRepository)
        {
            ResourceCounts = resourceCounts;
            AborigineRepository = aborigineRepository;
        }

        public ResourceCounts ResourceCounts { get; }
        public AborigineRepository AborigineRepository { get; }
    }
}
