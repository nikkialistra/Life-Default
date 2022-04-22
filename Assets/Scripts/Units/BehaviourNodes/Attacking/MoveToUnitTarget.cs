using BehaviorDesigner.Runtime.Tasks;
using Units.BehaviorVariables;

namespace Units.BehaviourNodes.Attacking
{
    public class MoveToUnitTarget : Action
    {
        public SharedUnit Unit;

        public UnitMeshAgent UnitMeshAgent;
        public UnitAttacker UnitAttacker;

        public override TaskStatus OnUpdate()
        {
            if (!UnitMeshAgent.IsMoving && !UnitAttacker.OnAttackRange(Unit.Value.transform.position))
            {
                UnitMeshAgent.SetDestinationToUnitTarget(Unit.Value, UnitAttacker.AttackRange);
            }

            return Unit.Value.Alive ? TaskStatus.Running : TaskStatus.Success;
        }
    }
}
