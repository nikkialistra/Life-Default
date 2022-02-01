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
        public UnitRole UnitRole;

        public override void OnStart()
        {
            UnitMeshAgent.SetDestinationToEntity(Enemy.Value.Entity, UnitRole.GetInteractionDistanceWithEnemies());
        }

        public override TaskStatus OnUpdate()
        {
            if (!UnitMeshAgent.IsMoving && OutOfAttackRange())
            {
                UnitMeshAgent.SetDestinationToEntity(Enemy.Value.Entity,
                    UnitRole.GetInteractionDistanceWithEnemies());
            }

            return Enemy.Value.Alive ? TaskStatus.Running : TaskStatus.Success;
        }

        private bool OutOfAttackRange()
        {
            return Vector3.Distance(transform.position, Enemy.Value.transform.position) >
                   UnitRole.GetAttackRangeDistanceWithEnemies();
        }
    }
}
