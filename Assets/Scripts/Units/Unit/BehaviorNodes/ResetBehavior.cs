using BehaviorDesigner.Runtime;
using BehaviorDesigner.Runtime.Tasks;
using Units.Unit.UnitTypes;

namespace Units.Unit.BehaviorNodes
{
    public class ResetBehavior : Action
    {
        public SharedBool NewCommand;

        public UnitClass UnitClass;

        public override TaskStatus OnUpdate()
        {
            NewCommand.Value = false;

            UnitClass.StopInteraction();

            return TaskStatus.Success;
        }
    }
}
