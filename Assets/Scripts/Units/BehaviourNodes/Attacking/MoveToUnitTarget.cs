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

        public override void OnStart()
        {
            UnitAttacker.SetTrackedUnit(UnitTarget.Value);
            UnitEquipment.EquipWeapon();
        }

        public override TaskStatus OnUpdate()
        {
            if (!UnitTarget.Value.Alive)
            {
                UnitTarget.Value = null;
                UnitMeshAgent.ResetDestination();
                return TaskStatus.Failure;
            }

            if (UnitAttacker.OnAttackDistance(UnitTarget.Value.transform.position))
            {
                return TaskStatus.Success;
            }

            if (!UnitMeshAgent.IsMoving)
            {
                UnitMeshAgent.SetDestinationToUnitTarget(UnitTarget.Value, UnitAttacker.AttackDistance);
            }

            return TaskStatus.Running;
        }
    }
}
