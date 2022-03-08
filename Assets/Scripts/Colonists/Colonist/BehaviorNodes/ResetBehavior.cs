using BehaviorDesigner.Runtime;
using BehaviorDesigner.Runtime.Tasks;

namespace Colonists.Colonist.BehaviorNodes
{
    public class ResetBehavior : Action
    {
        public SharedBool NewCommand;

        public ColonistMeshAgent ColonistMeshAgent;

        public override TaskStatus OnUpdate()
        {
            NewCommand.Value = false;

            ColonistMeshAgent.StopMoving();

            return TaskStatus.Success;
        }
    }
}
