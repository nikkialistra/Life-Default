using BehaviorDesigner.Runtime.Tasks;
using Entities.BehaviorVariables;
using Units.Unit.UnitTypes;

namespace Units.Unit.BehaviorNodes
{
    public class AttackEnemy : Action
    {
        public SharedEnemy Enemy;

        public UnitMeshAgent UnitMeshAgent;
        public UnitRole UnitRole;

        private bool _interacting;

        public override TaskStatus OnUpdate()
        {
            if (!Enemy.Value.Alive)
            {
                Enemy.Value = null;
                return TaskStatus.Failure;
            }

            if (!_interacting && UnitRole.OnAttackRange(Enemy.Value.transform.position))
            {
                _interacting = true;
                UnitRole.InteractWith(Enemy.Value, OnInteractionFinish);
            }

            return TaskStatus.Running;
        }

        private void OnInteractionFinish()
        {
            _interacting = false;
        }

        public override void OnEnd()
        {
            UnitMeshAgent.StopRotating();
        }
    }
}
