using BehaviorDesigner.Runtime.Tasks;
using Entities.BehaviorVariables;

namespace Colonists.Colonist.BehaviorNodes.ResourceGathering
{
    public class MoveToResource : Action
    {
        public float InteractionDistance = 2f;

        public SharedResource Resource;

        public ColonistMeshAgent ColonistMeshAgent;

        private bool _finished;

        public override void OnStart()
        {
            _finished = false;

            ColonistMeshAgent.DestinationReach += OnDestinationReach;
            ColonistMeshAgent.SetDestinationToResource(Resource.Value, InteractionDistance);
        }

        public override TaskStatus OnUpdate()
        {
            return _finished ? TaskStatus.Success : TaskStatus.Running;
        }

        private void OnDestinationReach()
        {
            ColonistMeshAgent.DestinationReach -= OnDestinationReach;
            _finished = true;
        }
    }
}
