using BehaviorDesigner.Runtime.Tasks;
using Units.BehaviorVariables;

namespace Units.BehaviourNodes.Attacking
{
    public class GotNewUnitTarget : Conditional
    {
        public SharedUnit Unit;

        public override TaskStatus OnUpdate()
        {
            return Unit.Value != null ? TaskStatus.Success : TaskStatus.Failure;
        }
    }
}
