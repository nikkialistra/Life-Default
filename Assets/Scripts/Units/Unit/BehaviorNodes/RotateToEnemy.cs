using BehaviorDesigner.Runtime.Tasks;
using Entities.Entity;
using Units.Unit.UnitTypes;
using UnityEngine;

namespace Units.Unit.BehaviorNodes
{
    public class RotateToEnemy : Action
    {
        public SharedEnemy Enemy;

        public UnitMeshAgent UnitMeshAgent;
        public UnitRole UnitRole;

        private bool _finished;

        public override TaskStatus OnUpdate()
        {
            if (!UnitMeshAgent.IsRotating && OnAttackRange())
            {
                UnitMeshAgent.RotateTo(Enemy.Value.Entity);
            }

            return TaskStatus.Running;
        }

        public override void OnEnd()
        {
            UnitMeshAgent.StopRotating();
        }

        private bool OnAttackRange()
        {
            return Vector3.Distance(transform.position, Enemy.Value.transform.position) <=
                   UnitRole.GetAttackRangeDistanceWithEnemies();
        }
    }
}
