using BehaviorDesigner.Runtime.Tasks;
using Units.BehaviorVariables;

namespace Units.BehaviourNodes.Attacking
{
    public class AttackUnitTarget : Action
    {
        public SharedUnit UnitTarget;

        public UnitMeshAgent UnitMeshAgent;
        public UnitAttacker UnitAttacker;

        public override TaskStatus OnUpdate()
        {
            if (!UnitTarget.Value.Alive)
            {
                UnitTarget.Value = null;
                UnitMeshAgent.ResetDestination();
                return TaskStatus.Failure;
            }
            
            if (!UnitAttacker.OnAttackRange(UnitTarget.Value.transform.position))
            {
                UnitAttacker.FinishAttacking();
                return TaskStatus.Success;
            }

            if (!UnitAttacker.IsAttacking)
            {
                UnitAttacker.Attack(UnitTarget.Value);
            }

            return TaskStatus.Running;
        }
    }
}
