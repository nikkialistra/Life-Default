using BehaviorDesigner.Runtime.Tasks;
using Entities.BehaviorVariables;
using Units.Unit.UnitTypes;

namespace Units.Unit.BehaviorNodes
{
    public class MoveToEnemy : Action
    {
        public SharedEnemy Enemy;

        public UnitMeshAgent UnitMeshAgent;
        public UnitRole UnitRole;

        public override TaskStatus OnUpdate()
        {
            if (!UnitMeshAgent.IsMoving && !UnitRole.OnAttackRange(Enemy.Value.transform.position))
            {
                UnitMeshAgent.SetDestinationToEnemy(Enemy.Value,
                    UnitRole.GetInteractionDistanceWithEnemies());
            }

            return Enemy.Value.Alive ? TaskStatus.Running : TaskStatus.Success;
        }
    }
}
