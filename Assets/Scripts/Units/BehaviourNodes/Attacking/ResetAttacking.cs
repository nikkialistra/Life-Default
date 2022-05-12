using BehaviorDesigner.Runtime.Tasks;
using Units.BehaviorVariables;

namespace Units.BehaviourNodes.Attacking
{
    public class ResetAttacking : Action
    {
        public SharedUnit UnitTarget;
        
        public UnitMeshAgent UnitMeshAgent;

        public override TaskStatus OnUpdate()
        {
            UnitMeshAgent.ResetDestination();
            UnitTarget.Value = null;
            return TaskStatus.Success;
        }
    }
}
