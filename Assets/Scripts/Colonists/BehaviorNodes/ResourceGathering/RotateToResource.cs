using BehaviorDesigner.Runtime.Tasks;
using Units.BehaviorVariables;

namespace Colonists.BehaviorNodes.ResourceGathering
{
    public class RotateToResource : Action
    {
        public SharedResource Resource;

        public ColonistMeshAgent ColonistMeshAgent;
        public ColonistGatherer ColonistGatherer;

        private bool _rotationEnd;

        public override void OnStart()
        {
            _rotationEnd = false;

            ColonistMeshAgent.RotationEnd += OnRotationEnd;
            ColonistMeshAgent.RotateTo(Resource.Value.Entity);
        }

        public override TaskStatus OnUpdate()
        {
            if (Resource.Value.Exhausted)
            {
                Resource.Value = null;
                ColonistMeshAgent.StopRotating();
                return TaskStatus.Failure;
            }

            return !_rotationEnd ? TaskStatus.Running : CanGather();
        }

        private TaskStatus CanGather()
        {
            return ColonistGatherer.AtInteractionDistance(Resource.Value) ? TaskStatus.Success : TaskStatus.Failure;
        }

        public override void OnEnd()
        {
            ColonistMeshAgent.StopRotating();
        }

        private void OnRotationEnd()
        {
            ColonistMeshAgent.RotationEnd -= OnRotationEnd;
            _rotationEnd = true;
        }
    }
}
