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
            
            if (!_rotationEnd)
            {
                return TaskStatus.Running;
            }
            else
            {
                return CanGather();
            }
        }
        
        private TaskStatus CanGather()
        {
            if (ColonistGatherer.CanGather(Resource.Value))
            {
                return TaskStatus.Success;
            }
            else
            {
                return TaskStatus.Failure;
            }
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
