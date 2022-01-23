using NPBehave;

namespace Enemies.Enemy.BehaviorNodes
{
    public class WalkToRandomLocation : Node
    {
        private readonly EnemyMeshAgent _enemyMeshAgent;
        private readonly float _walkRadius;

        public WalkToRandomLocation(EnemyMeshAgent enemyMeshAgent, float walkRadius) : base("WalkToRandomLocation")
        {
            _enemyMeshAgent = enemyMeshAgent;
            _walkRadius = walkRadius;
        }

        protected override void DoStart()
        {
            _enemyMeshAgent.GoInRadius(_walkRadius);

            Stopped(true);
        }

        protected override void DoStop()
        {
            Stopped(false);
        }
    }
}
