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
            UnitMeshAgent.SetDestinationToEntity(Enemy.Value.Entity);
        }

        public override TaskStatus OnUpdate()
        {
            if (OnInteractionDistance() && UnitMeshAgent.IsMoving)
            {
                UnitMeshAgent.StopMoving();
            }

            if (!OnAttackRangeDistance() && !UnitMeshAgent.IsMoving)
            {
                UnitMeshAgent.SetDestinationToEntity(Enemy.Value.Entity);
            }

            return Enemy.Value.Alive ? TaskStatus.Running : TaskStatus.Success;
        }

        private bool OnInteractionDistance()
        {
            return Vector3.Distance(transform.position, Enemy.Value.transform.position) <
                   UnitClass.GetInteractionDistanceWith(Enemy.Value) - 0.5f;
        }

        private bool OnAttackRangeDistance()
        {
            return Vector3.Distance(transform.position, Enemy.Value.transform.position) <
                   UnitClass.GetInteractionDistanceWith(Enemy.Value);
        }
    }
}
