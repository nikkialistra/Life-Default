using BehaviorDesigner.Runtime.Tasks;
using Units.BehaviorVariables;

namespace Colonists.BehaviorNodes.Moving
{
    public class MoveToPosition : Action
    {
        public SharedPositions Positions;

        public ColonistMeshAgent ColonistMeshAgent;

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

            ColonistMeshAgent.DestinationReach -= OnDestinationReach;
            ColonistMeshAgent.DestinationReach += OnDestinationReach;
            ColonistMeshAgent.SetDestinationToPosition(Positions.Value.Peek());
        }

        public override TaskStatus OnUpdate()
        {
            if (_end)
                return TaskStatus.Failure;

            return _finished ? TaskStatus.Success : TaskStatus.Running;
        }

        private void OnDestinationReach()
        {
            if (Positions.Value.Count > 0)
                Positions.Value.Dequeue();

            ColonistMeshAgent.DestinationReach -= OnDestinationReach;
            _finished = true;
        }
    }
}
