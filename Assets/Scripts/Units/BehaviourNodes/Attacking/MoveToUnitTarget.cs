using BehaviorDesigner.Runtime.Tasks;
using Units.BehaviorVariables;

namespace Units.BehaviourNodes.Attacking
{
    public class MoveToUnitTarget : Action
    {
        public SharedUnit UnitTarget;

        public UnitMeshAgent UnitMeshAgent;
        public UnitAttacker UnitAttacker;

        public override TaskStatus OnUpdate()
        {
            if (UnitTarget.Value == null)
            {
                return TaskStatus.Failure;
            }
            
            if (!UnitTarget.Value.Alive)
            {
                UnitTarget.Value = null;
                return TaskStatus.Failure;
            }
            
            if (UnitAttacker.OnAttackRange(UnitTarget.Value.transform.position))
            {
                return TaskStatus.Success;
            }

            if (!UnitMeshAgent.IsMoving && !UnitAttacker.OnAttackRange(UnitTarget.Value.transform.position))
            {
                UnitMeshAgent.SetDestinationToUnitTarget(UnitTarget.Value, UnitAttacker.AttackRange);
            }

            return TaskStatus.Running;
        }
    }
}
