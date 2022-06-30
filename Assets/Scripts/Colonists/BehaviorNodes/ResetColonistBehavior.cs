using BehaviorDesigner.Runtime;
using BehaviorDesigner.Runtime.Tasks;
using Units;

namespace Colonists.BehaviorNodes
{
    public class ResetColonistBehavior : Action
    {
        public SharedBool NewCommand;

        public ColonistMeshAgent ColonistMeshAgent;
        public ColonistGatherer ColonistGatherer;

        public UnitAttacker UnitAttacker;

        public override TaskStatus OnUpdate()
        {
            NewCommand.Value = false;

            ColonistMeshAgent.StopMoving();
            ColonistMeshAgent.ResetDestination();
            ColonistGatherer.FinishGathering();

            UnitAttacker.FinalizeAttackingInstantly();

            return TaskStatus.Success;
        }
    }
}
