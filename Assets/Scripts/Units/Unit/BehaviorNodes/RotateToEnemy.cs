using BehaviorDesigner.Runtime.Tasks;
using Entities.BehaviorVariables;
using Units.Unit.UnitTypes;

namespace Units.Unit.BehaviorNodes
{
    public class RotateToEnemy : Action
    {
        public SharedEnemy Enemy;

        public UnitMeshAgent UnitMeshAgent;
        public UnitRole UnitRole;

        private bool _finished;

        public override TaskStatus OnUpdate()
        {
            if (!UnitMeshAgent.IsRotating && UnitRole.OnAttackRange(Enemy.Value.transform.position))
            {
                UnitMeshAgent.RotateTo(Enemy.Value.Entity);
            }

            return TaskStatus.Running;
        }

        public override void OnEnd()
        {
            UnitMeshAgent.StopRotating();
        }
    }
}
