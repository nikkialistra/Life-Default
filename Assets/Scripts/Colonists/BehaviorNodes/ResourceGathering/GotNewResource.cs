using BehaviorDesigner.Runtime.Tasks;
using Units.BehaviorVariables;

namespace Colonists.BehaviorNodes.ResourceGathering
{
    public class GotNewResource : Conditional
    {
        public SharedResource Resource;

        public override TaskStatus OnUpdate()
        {
            return Resource.Value != null ? TaskStatus.Success : TaskStatus.Failure;
        }
    }
}
