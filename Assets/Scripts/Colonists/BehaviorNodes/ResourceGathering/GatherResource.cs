using BehaviorDesigner.Runtime.Tasks;
using Units.BehaviorVariables;
using UnityEngine;

namespace Colonists.BehaviorNodes.ResourceGathering
{
    public class GatherResource : Action
    {
        public SharedResource Resource;

        public ColonistGatherer ColonistGatherer;

        private bool _failed;

        public override void OnStart()
        {
            _failed = true;
        
            if (ColonistGatherer.CanGather(Resource.Value) && !ColonistGatherer.IsGathering)
            {
                ColonistGatherer.Gather(Resource.Value);
            }
        }

        public override TaskStatus OnUpdate()
        {
            if (_failed)
            {
                return TaskStatus.Failure;
            }

            return ColonistGatherer.IsGathering ? TaskStatus.Running : TaskStatus.Success;
        }
    }
}
