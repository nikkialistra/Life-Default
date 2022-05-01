using BehaviorDesigner.Runtime;
using BehaviorDesigner.Runtime.Tasks;
using Units;

namespace Enemies.BehaviorNodes
{
    public class ResetEnemyBehavior : Action
    {
        public SharedBool NewCommand;

        public EnemyMeshAgent EnemyMeshAgent;
        
        public UnitAttacker UnitAttacker;

        public override TaskStatus OnUpdate()
        {
            NewCommand.Value = false;

            EnemyMeshAgent.StopMoving();

            UnitAttacker.FinishAttacking();

            return TaskStatus.Success;
        }
    }
}
