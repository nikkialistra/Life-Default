using BehaviorDesigner.Runtime.Tasks;
using Units.BehaviorVariables;

namespace Units.BehaviourNodes.Attacking
{
    public class ResetAttacking : Action
    {
        public SharedUnit UnitTarget;

        public UnitMeshAgent UnitMeshAgent;
        public UnitAttacker UnitAttacker;
        public UnitEquipment UnitEquipment;

        public override TaskStatus OnUpdate()
        {
            UnitMeshAgent.StopRotating();
            UnitMeshAgent.ResetDestination();

            UnitAttacker.CoverUnitTarget();
            UnitEquipment.UnequipWeapon();

            UnitTarget.Value = null;

            return TaskStatus.Success;
        }
    }
}
