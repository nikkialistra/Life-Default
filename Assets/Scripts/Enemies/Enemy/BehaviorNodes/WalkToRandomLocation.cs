using NPBehave;

namespace Enemies.Enemy.BehaviorNodes
{
    public class WalkToRandomLocation : Node
    {
        private readonly EnemyMeshAgent _enemyMeshAgent;

        private float _walkMinRadius;
        private readonly float _walkMaxRadius;

        public WalkToRandomLocation(EnemyMeshAgent enemyMeshAgent, float walkMinRadius, float walkMaxRadius) : base(
            "WalkToRandomLocation")
        {
            _enemyMeshAgent = enemyMeshAgent;

            _walkMinRadius = walkMinRadius;
            _walkMaxRadius = walkMaxRadius;
        }

        protected override void DoStart()
        {
            _enemyMeshAgent.GoInRadius(_walkMinRadius, _walkMaxRadius);

            Stopped(true);
        }

        protected override void DoStop()
        {
            Stopped(false);
        }
    }
}
