using BehaviorDesigner.Runtime.Tasks;
using Units.BehaviorVariables;

namespace Units.BehaviourNodes.Attacking
{
    public class GotNewUnitTarget : Conditional
    {
        public SharedUnit UnitTarget;

        public override TaskStatus OnUpdate()
        {
            return UnitTarget.Value != null ? TaskStatus.Success : TaskStatus.Failure;
        }
    }
}
