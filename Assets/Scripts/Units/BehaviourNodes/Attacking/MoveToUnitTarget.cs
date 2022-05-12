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
                return TaskStatus.Failure;
            }

            if (!UnitMeshAgent.IsMoving && OutOfAttackDistance)
            {
                UnitMeshAgent.SetDestinationToUnitTarget(UnitTarget.Value, UnitAttacker.AttackDistance);
            }

            return TaskStatus.Running;
        }

        private bool OutOfAttackDistance => !UnitAttacker.OnAttackDistance(UnitTarget.Value.transform.position);
    }
}
