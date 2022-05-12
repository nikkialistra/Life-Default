using BehaviorDesigner.Runtime.Tasks;
using Units.BehaviorVariables;
using UnityEngine;

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
                return TaskStatus.Failure;
            }
            
            if (OutOfReach(UnitTarget.Value.transform.position))
            {
                if (UnitAttacker.IsAttacking)
                {
                    UnitAttacker.FinalizeAttacking();
                }
                
                return TaskStatus.Running;
            }

            if (!UnitAttacker.IsAttacking)
            {
                UnitAttacker.Attack(UnitTarget.Value);
            }

            return TaskStatus.Running;
        }

        private bool OutOfReach(Vector3 position)
        {
            return !UnitAttacker.OnAttackRange(position) || !UnitAttacker.OnAttackAngle(position);
        }
    }
}
