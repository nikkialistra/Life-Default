using BehaviorDesigner.Runtime.Tasks;
using Units.BehaviorVariables;

namespace Units.BehaviourNodes.Attacking
{
    public class ResetAttacking : Action
    {
        public SharedUnit UnitTarget;
        
        public UnitMeshAgent UnitMeshAgent;
        public UnitAttacker UnitAttacker;

        public override TaskStatus OnUpdate()
        {
            UnitMeshAgent.ResetDestination();
            UnitAttacker.FinalizeAttackingInstantly();
            UnitTarget.Value = null;
            return TaskStatus.Success;
        }
    }
}
