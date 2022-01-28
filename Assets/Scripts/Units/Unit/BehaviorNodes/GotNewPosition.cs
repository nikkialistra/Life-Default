using BehaviorDesigner.Runtime;
using BehaviorDesigner.Runtime.Tasks;

namespace Units.Unit.BehaviorNodes
{
    public class GotNewPosition : Conditional
    {
        public SharedVector3 Position;

        public override TaskStatus OnUpdate()
        {
            return float.IsNegativeInfinity(Position.Value.x) ? TaskStatus.Failure : TaskStatus.Success;
        }
    }
}
