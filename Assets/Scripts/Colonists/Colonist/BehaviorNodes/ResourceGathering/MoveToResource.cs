using BehaviorDesigner.Runtime.Tasks;
using Entities.BehaviorVariables;
using Action = BehaviorDesigner.Runtime.Tasks.Action;

namespace Colonists.Colonist.BehaviorNodes.ResourceGathering
{
    public class MoveToResource : Action
    {
        public SharedResource Resource;

        public ColonistMeshAgent ColonistMeshAgent;
        public ColonistGatherer ColonistGatherer;

        private bool _finished;

        public override void OnStart()
        {
            _finished = false;

            var interactionDistance = ColonistGatherer.InteractionDistanceFor(Resource.Value.ResourceType);

            ColonistMeshAgent.DestinationReach += OnDestinationReach;
            ColonistMeshAgent.SetDestinationToResource(Resource.Value, interactionDistance);
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
