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
            if (!UnitMeshAgent.IsRotating && UnitAttacker.OnAttackRange(UnitTarget.Value.transform.position))
            {
                UnitMeshAgent.RotateTo(UnitTarget.Value.transform.position);
            }

            return TaskStatus.Running;
        }

        public override void OnEnd()
        {
            UnitMeshAgent.StopRotating();
        }
    }
}
