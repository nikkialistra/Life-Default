using BehaviorDesigner.Runtime.Tasks;
using Entities.BehaviorVariables;

namespace Units.Unit.BehaviorNodes
{
    public class GotNewEnemy : Conditional
    {
        public SharedEnemy Enemy;

        public override TaskStatus OnUpdate()
        {
            return Enemy.Value != null ? TaskStatus.Success : TaskStatus.Failure;
        }
    }
}
