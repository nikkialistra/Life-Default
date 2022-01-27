using BehaviorDesigner.Runtime;
using BehaviorDesigner.Runtime.Tasks;
using UnityEngine;

namespace Units.Unit.BehaviorNodes
{
    public class GotNewPosition : Conditional
    {
        public SharedVector3 Position;

        public override TaskStatus OnUpdate()
        {
            return Position.Value != Vector3.negativeInfinity ? TaskStatus.Success : TaskStatus.Failure;
        }
    }
}
