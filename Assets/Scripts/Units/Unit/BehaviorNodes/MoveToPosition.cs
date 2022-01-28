using BehaviorDesigner.Runtime;
using BehaviorDesigner.Runtime.Tasks;
using UnityEngine;

namespace Units.Unit.BehaviorNodes
{
    public class MoveToPosition : Action
    {
        public SharedVector3 Position;

        public UnitMeshAgent UnitMeshAgent;

        private bool _finished;

        public override void OnStart()
        {
            _finished = false;

            UnitMeshAgent.DestinationReach += OnDestinationReach;
            UnitMeshAgent.SetDestinationToPosition(Position.Value);
        }

        public override TaskStatus OnUpdate()
        {
            return _finished ? TaskStatus.Success : TaskStatus.Running;
        }

        private void OnDestinationReach()
        {
            UnitMeshAgent.DestinationReach -= OnDestinationReach;
            Position.Value = Vector3.negativeInfinity;
            _finished = true;
        }
    }
}
