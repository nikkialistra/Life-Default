using BehaviorDesigner.Runtime;
using BehaviorDesigner.Runtime.Tasks;
using Units;

namespace Colonists.BehaviorNodes
{
    public class ResetBehavior : Action
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
            
            UnitAttacker.FinishAttacking();

            return TaskStatus.Success;
        }
    }
}
