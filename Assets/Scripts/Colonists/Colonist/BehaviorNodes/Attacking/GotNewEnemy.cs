using BehaviorDesigner.Runtime.Tasks;
using Entities.BehaviorVariables;

namespace Colonists.Colonist.BehaviorNodes.Attacking
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
