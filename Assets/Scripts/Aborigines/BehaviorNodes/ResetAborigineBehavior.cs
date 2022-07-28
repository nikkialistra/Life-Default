using BehaviorDesigner.Runtime;
using BehaviorDesigner.Runtime.Tasks;
using Units;

namespace Aborigines.BehaviorNodes
{
    public class ResetAborigineBehavior : Action
    {
        public SharedBool NewCommand;

        public AborigineMeshAgent AborigineMeshAgent;

        public UnitAttacker UnitAttacker;

        public override TaskStatus OnUpdate()
        {
            NewCommand.Value = false;

            AborigineMeshAgent.StopMoving();

            UnitAttacker.FinalizeAttacking();

            return TaskStatus.Success;
        }
    }
}
