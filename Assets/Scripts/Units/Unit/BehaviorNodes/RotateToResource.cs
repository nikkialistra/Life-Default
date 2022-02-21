using BehaviorDesigner.Runtime.Tasks;
using Entities.BehaviorVariables;

namespace Units.Unit.BehaviorNodes
{
    public class RotateToResource : Action
    {
        public SharedResource Resource;

        public UnitMeshAgent UnitMeshAgent;

        private bool _finished;
        private bool _failed;

        public override void OnStart()
        {
            _finished = false;
            _failed = false;

            if (Resource.Value.Entity == null)
            {
                _failed = true;
                return;
            }

            UnitMeshAgent.RotationEnd += OnRotationEnd;
            UnitMeshAgent.RotateTo(Resource.Value.Entity);
        }

        public override TaskStatus OnUpdate()
        {
            if (_failed)
            {
                return TaskStatus.Failure;
            }

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
