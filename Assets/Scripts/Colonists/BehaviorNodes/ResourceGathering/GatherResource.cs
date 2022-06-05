using BehaviorDesigner.Runtime.Tasks;
using Colonists.Activities;
using Units.BehaviorVariables;

namespace Colonists.BehaviorNodes.ResourceGathering
{
    public class GatherResource : Action
    {
        public SharedResource Resource;

        public ColonistGatherer ColonistGatherer;
        public ColonistActivities ColonistActivities;

        private bool _gathering;

        public override void OnStart()
        {
            _gathering = ColonistGatherer.TryGather(Resource.Value);

            if (_gathering)
            {
                ColonistActivities.SwitchTo(ActivityType.Gathering);
            }
        }

        public override TaskStatus OnUpdate()
        {
            if (!_gathering)
            {
                return TaskStatus.Failure;
            }
            
            if (Resource.Value.Exhausted)
            {
                return TaskStatus.Success;
            }

            if (ColonistGatherer.IsGathering)
            {
                return TaskStatus.Running;
            }
            else
            {
                return TaskStatus.Failure;
            }
        }
    }
}
