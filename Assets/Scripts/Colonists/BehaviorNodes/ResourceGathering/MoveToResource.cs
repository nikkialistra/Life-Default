using BehaviorDesigner.Runtime.Tasks;
using Units.BehaviorVariables;
using Action = BehaviorDesigner.Runtime.Tasks.Action;

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

            if (!_destinationReached)
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

        private void OnDestinationReach()
        {
            ColonistMeshAgent.DestinationReach -= OnDestinationReach;
            _destinationReached = true;
        }
    }
}
