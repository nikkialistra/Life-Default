using BehaviorDesigner.Runtime.Tasks;
using Entities.Entity;
using Units.Unit.UnitTypes;
using UnityEngine;

namespace Units.Unit.BehaviorNodes
{
    public class MoveToEnemy : Action
    {
        public SharedEnemy Enemy;

        public UnitMeshAgent UnitMeshAgent;
        public UnitClass UnitClass;

        public override void OnStart()
        {
            UnitMeshAgent.SetDestinationToEntity(Enemy.Value.Entity, UnitClass.GetInteractionDistanceWith(Enemy.Value));
        }

        public override TaskStatus OnUpdate()
        {
            if (!UnitMeshAgent.IsMoving && !OnAttackRangeDistance())
            {
                UnitMeshAgent.SetDestinationToEntity(Enemy.Value.Entity,
                    UnitClass.GetInteractionDistanceWith(Enemy.Value));
            }

            return Enemy.Value.Alive ? TaskStatus.Running : TaskStatus.Success;
        }

        private bool OnAttackRangeDistance()
        {
            return Vector3.Distance(transform.position, Enemy.Value.transform.position) <
                   UnitClass.GetAttackRangeDistanceWith(Enemy.Value);
        }
    }
}
