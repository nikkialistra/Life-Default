using BehaviorDesigner.Runtime.Tasks;
using Units.BehaviorVariables;

namespace Colonists.BehaviorNodes.ResourceGathering
{
    public class GatherResource : Action
    {
        public SharedResource Resource;

        public ColonistGatherer ColonistGatherer;

        private bool _gathering;

        public override void OnStart()
        {
            _gathering = ColonistGatherer.TryGather(Resource.Value);
        }

        public override TaskStatus OnUpdate()
        {
            if (!_gathering)
            {
                return TaskStatus.Failure;
            }

            if (ColonistGatherer.IsGathering)
            {
                return TaskStatus.Running;
            }
            else
            {
                if (Resource.Value.Exhausted)
                {
                    return TaskStatus.Success;
                }
                else
                {
                    return TaskStatus.Failure;
                }
            }
        }
    }
}
