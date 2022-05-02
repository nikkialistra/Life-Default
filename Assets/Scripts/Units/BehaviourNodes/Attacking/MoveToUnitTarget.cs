using BehaviorDesigner.Runtime.Tasks;
using Units.BehaviorVariables;
using UnityEngine;

namespace Units.BehaviourNodes.Attacking
{
    public class MoveToUnitTarget : Action
    {
        public SharedUnit UnitTarget;

        public UnitMeshAgent UnitMeshAgent;
        public UnitAttacker UnitAttacker;
        public UnitEquipment UnitEquipment;

        public override TaskStatus OnUpdate()
        {
            if (!UnitTarget.Value.Alive)
            {
                UnitTarget.Value = null;
                UnitMeshAgent.ResetDestination();
                return TaskStatus.Failure;
            }

            UnitAttacker.SetTrackedUnit(UnitTarget.Value);
            UnitEquipment.EquipWeapon();

            if (UnitAttacker.OnAttackRange(UnitTarget.Value.transform.position))
            {
                return TaskStatus.Success;
            }

            if (!UnitMeshAgent.IsMoving)
            {
                UnitMeshAgent.SetDestinationToUnitTarget(UnitTarget.Value, UnitAttacker.AttackRange);
            }

            return TaskStatus.Running;
        }
    }
}
