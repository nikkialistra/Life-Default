using BehaviorDesigner.Runtime.Tasks;
using Units.BehaviorVariables;

namespace Units.BehaviourNodes.Attacking
{
    public class RotateToUnitTarget : Action
    {
        public float AngleDifferenceToRotate = 3f;
        
        public SharedUnit UnitTarget;

        public UnitMeshAgent UnitMeshAgent;
        public UnitAttacker UnitAttacker;

        public override TaskStatus OnUpdate()
        {
            if (!UnitTarget.Value.Alive)
            {
                return TaskStatus.Failure;
            }

            if (UnitAttacker.OnAttackRange(UnitTarget.Value.transform.position) &&
                UnitMeshAgent.GetAngleDifferenceWith(UnitTarget.Value.transform.position) > AngleDifferenceToRotate)
            {
                UnitMeshAgent.RotateTo(UnitTarget.Value.transform.position);
            }

            return TaskStatus.Running;
        }
    }
}
