using BehaviorDesigner.Runtime.Tasks;
using Units.BehaviorVariables;

namespace Units.BehaviourNodes.Attacking
{
    public class AttackUnitTarget : Action
    {
        public SharedUnit UnitTarget;

        public UnitMeshAgent UnitMeshAgent;
        public UnitAttacker UnitAttacker;

        private bool _interacting;

        public override void OnStart()
        {
            _interacting = false;
        }

        public override TaskStatus OnUpdate()
        {
            if (!UnitTarget.Value.Alive)
            {
                UnitTarget.Value = null;
                return TaskStatus.Failure;
            }

            if (!_interacting && UnitAttacker.OnAttackRange(UnitTarget.Value.transform.position))
            {
                _interacting = true;
                UnitAttacker.Attack(UnitTarget.Value, OnInteractionFinish);
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
