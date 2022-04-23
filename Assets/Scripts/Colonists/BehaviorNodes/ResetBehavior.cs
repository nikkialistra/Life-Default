using BehaviorDesigner.Runtime;
using BehaviorDesigner.Runtime.Tasks;

namespace Colonists.BehaviorNodes
{
    public class ResetBehavior : Action
    {
        public SharedBool NewCommand;

        public ColonistMeshAgent ColonistMeshAgent;
        public ColonistGatherer ColonistGatherer;

        public override TaskStatus OnUpdate()
        {
            NewCommand.Value = false;

            ColonistMeshAgent.StopMoving();
            ColonistGatherer.FinishGathering();

            return TaskStatus.Success;
        }
    }
}
