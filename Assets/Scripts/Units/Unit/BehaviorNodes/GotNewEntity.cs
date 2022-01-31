using BehaviorDesigner.Runtime.Tasks;
using Entities.Entity;

namespace Units.Unit.BehaviorNodes
{
    public class GotNewEntity : Conditional
    {
        public SharedResource Resource;

        public override TaskStatus OnUpdate()
        {
            return Resource.Value != null ? TaskStatus.Success : TaskStatus.Failure;
        }
    }
}
