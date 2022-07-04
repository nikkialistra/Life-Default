using BehaviorDesigner.Runtime.Tasks;
using Units.BehaviorVariables;

namespace Colonists.BehaviorNodes.ResourceGathering
{
    public class MoveToResource : Action
    {
        public SharedResource Resource;

        public ColonistMeshAgent ColonistMeshAgent;
        public ColonistGatherer ColonistGatherer;

        private bool _destinationReached;

        public override void OnStart()
        {
            _destinationReached = false;

            var interactionDistance = ColonistGatherer.InteractionDistanceFor(Resource.Value.ResourceType);

            ColonistMeshAgent.DestinationReach += OnDestinationReach;
            ColonistMeshAgent.SetDestinationToResource(Resource.Value, interactionDistance);
        }

        public override TaskStatus OnUpdate()
        {
            if (Resource.Value.Exhausted)
            {
                Resource.Value = null;
                ColonistMeshAgent.ResetDestination();
                return TaskStatus.Failure;
            }

            return !_destinationReached ? TaskStatus.Running : AtInteractionDistance();
        }

        private TaskStatus AtInteractionDistance()
        {
            return ColonistGatherer.AtInteractionDistance(Resource.Value) ? TaskStatus.Success : TaskStatus.Failure;
        }

        private void OnDestinationReach()
        {
            ColonistMeshAgent.DestinationReach -= OnDestinationReach;
            _destinationReached = true;
        }
    }
}
