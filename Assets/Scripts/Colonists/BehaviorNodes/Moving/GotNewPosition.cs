using BehaviorDesigner.Runtime.Tasks;
using Entities.BehaviorVariables;

namespace Colonists.BehaviorNodes.Moving
{
    public class GotNewPosition : Conditional
    {
        public SharedPositions Positions;

        public override TaskStatus OnUpdate()
        {
            return Positions.Value.Count > 0 ? TaskStatus.Success : TaskStatus.Failure;
        }
    }
}
