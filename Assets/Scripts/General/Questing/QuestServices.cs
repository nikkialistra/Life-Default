using Enemies.Services;
using ResourceManagement;

namespace General.Questing
{
    public class QuestServices
    {
        public QuestServices(ResourceCounts resourceCounts, EnemyRepository enemyRepository)
        {
            ResourceCounts = resourceCounts;
            EnemyRepository = enemyRepository;
        }

        public ResourceCounts ResourceCounts { get; }
        public EnemyRepository EnemyRepository { get; }
    }
}
