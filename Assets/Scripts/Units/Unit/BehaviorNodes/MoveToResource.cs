using BehaviorDesigner.Runtime.Tasks;
using Entities.Entity;

namespace Units.Unit.BehaviorNodes
{
    public class MoveToResource : Action
    {
        public float InteractionDistance = 2f;

        public SharedResource Resource;

        public UnitMeshAgent UnitMeshAgent;

        private bool _finished;

        public override void OnStart()
        {
            _finished = false;

            UnitMeshAgent.DestinationReach += OnDestinationReach;
            UnitMeshAgent.SetDestinationToResource(Resource.Value, InteractionDistance);
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
