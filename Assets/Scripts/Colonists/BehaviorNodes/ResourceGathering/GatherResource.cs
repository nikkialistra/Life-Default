using BehaviorDesigner.Runtime.Tasks;
using Entities.BehaviorVariables;

namespace Colonists.BehaviorNodes.ResourceGathering
{
    public class GatherResource : Action
    {
        public SharedResource Resource;

        public ColonistGatherer ColonistGatherer;

        private bool _finished;
        private bool _failed;

        public override void OnStart()
        {
            _finished = false;
            _failed = false;
        
            if (!ColonistGatherer.CanGather(Resource.Value))
            {
                _failed = true;
            }
            else
            {
                ColonistGatherer.Gather(Resource.Value, OnInteractionFinish);
            }
        }

        public override TaskStatus OnUpdate()
        {
            if (_failed)
            {
                return TaskStatus.Failure;
            }

            return _finished ? TaskStatus.Success : TaskStatus.Running;
        }

        private void OnInteractionFinish()
        {
            _finished = true;
        }
    }
}
