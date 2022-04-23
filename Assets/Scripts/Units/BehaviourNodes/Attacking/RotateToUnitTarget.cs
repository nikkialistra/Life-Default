using BehaviorDesigner.Runtime.Tasks;
using Units.BehaviorVariables;

namespace Units.BehaviourNodes.Attacking
{
    public class RotateToUnitTarget : Action
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
                return TaskStatus.Failure;
            }
            
            if (!UnitMeshAgent.IsRotating)
            {
                UnitMeshAgent.RotateTo(UnitTarget.Value.transform.position);
                return TaskStatus.Success;
            }

            return TaskStatus.Running;
        }

        public override void OnEnd()
        {
            UnitMeshAgent.StopRotating();
        }
    }
}
