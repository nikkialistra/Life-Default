using BehaviorDesigner.Runtime.Tasks;
using Entities.Entity;

namespace Units.Unit.BehaviorNodes
{
    public class GotNewEntity : Conditional
    {
        public SharedEntity Entity;

        public override TaskStatus OnUpdate()
        {
            return Entity.Value != null ? TaskStatus.Success : TaskStatus.Failure;
        }
    }
}
