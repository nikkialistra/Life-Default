using BehaviorDesigner.Runtime.Tasks;
using Entities.Entity;

namespace Units.Unit.BehaviorNodes
{
    public class RotateToEntity : Action
    {
        public SharedEntity Entity;

        public UnitMeshAgent UnitMeshAgent;

        private bool _finished;

        public override void OnStart()
        {
            _finished = false;

            UnitMeshAgent.RotationEnd += OnRotationEnd;
            UnitMeshAgent.RotateTo(Entity.Value);
        }

        public override TaskStatus OnUpdate()
        {
            return _finished ? TaskStatus.Success : TaskStatus.Running;
        }

        public override void OnEnd()
        {
            UnitMeshAgent.StopRotating();
        }

        private void OnRotationEnd()
        {
            UnitMeshAgent.RotationEnd -= OnRotationEnd;
            _finished = true;
        }
    }
}
