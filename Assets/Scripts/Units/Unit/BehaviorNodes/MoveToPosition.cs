using BehaviorDesigner.Runtime.Tasks;
using Entities.BehaviorVariables;

namespace Units.Unit.BehaviorNodes
{
    public class MoveToPosition : Action
    {
        public SharedPositions Positions;

        public UnitMeshAgent UnitMeshAgent;

        private bool _finished;
        private bool _end;

        public override void OnStart()
        {
            _finished = false;
            _end = false;

            if (Positions.Value.Count == 0)
            {
                _end = true;
                return;
            }

            UnitMeshAgent.DestinationReach -= OnDestinationReach;
            UnitMeshAgent.DestinationReach += OnDestinationReach;
            UnitMeshAgent.SetDestinationToPosition(Positions.Value.Peek());
        }

        public override TaskStatus OnUpdate()
        {
            if (_end)
            {
                return TaskStatus.Failure;
            }

            return _finished ? TaskStatus.Success : TaskStatus.Running;
        }

        private void OnDestinationReach()
        {
            if (Positions.Value.Count > 0)
            {
                Positions.Value.Dequeue();
            }

            UnitMeshAgent.DestinationReach -= OnDestinationReach;
            _finished = true;
        }
    }
}
