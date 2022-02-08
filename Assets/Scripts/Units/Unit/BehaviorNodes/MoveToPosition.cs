using BehaviorDesigner.Runtime.Tasks;
using Entities.BehaviorVariables;

namespace Units.Unit.BehaviorNodes
{
    public class MoveToPosition : Action
    {
        public SharedPositions Positions;

        public UnitMeshAgent UnitMeshAgent;

        private bool _finished;

        public override void OnStart()
        {
            _finished = false;

            UnitMeshAgent.DestinationReach += OnDestinationReach;
            UnitMeshAgent.SetDestinationToPosition(Positions.Value.Dequeue());
        }

        public override TaskStatus OnUpdate()
        {
            return _finished ? TaskStatus.Success : TaskStatus.Running;
        }

        private void OnDestinationReach()
        {
            UnitMeshAgent.DestinationReach -= OnDestinationReach;
            _finished = true;
        }
    }
}
