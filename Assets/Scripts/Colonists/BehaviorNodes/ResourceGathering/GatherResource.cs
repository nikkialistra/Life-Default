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
            _failed = false;
        
            if (ColonistGatherer.CanGather(Resource.Value))
            {
                if (!ColonistGatherer.IsGathering)
                {
                    ColonistGatherer.Gather(Resource.Value);
                }
            }
            else
            {
                _failed = true;
            }
        }

        public override TaskStatus OnUpdate()
        {
            if (_failed)
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
