using BehaviorDesigner.Runtime;
using BehaviorDesigner.Runtime.Tasks;
using Units.Unit.UnitTypes;

namespace Units.Unit.BehaviorNodes
{
    public class ResetBehavior : Action
    {
        public SharedBool NewCommand;

        public UnitMeshAgent UnitMeshAgent;
        public UnitRole UnitRole;

        public override TaskStatus OnUpdate()
        {
            NewCommand.Value = false;

            UnitMeshAgent.StopMoving();
            UnitRole.StopInteraction();

            return TaskStatus.Success;
        }
    }
}
