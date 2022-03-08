using BehaviorDesigner.Runtime.Tasks;
using Entities.BehaviorVariables;

namespace Colonists.Colonist.BehaviorNodes
{
    public class RotateToResource : Action
    {
        public SharedResource Resource;

        public ColonistMeshAgent ColonistMeshAgent;

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

            ColonistMeshAgent.RotationEnd += OnRotationEnd;
            ColonistMeshAgent.RotateTo(Resource.Value.Entity);
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
            ColonistMeshAgent.StopRotating();
        }

        private void OnRotationEnd()
        {
            ColonistMeshAgent.RotationEnd -= OnRotationEnd;
            _finished = true;
        }
    }
}
