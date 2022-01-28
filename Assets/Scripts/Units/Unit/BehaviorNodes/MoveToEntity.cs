using BehaviorDesigner.Runtime.Tasks;
using Entities.Entity;

namespace Units.Unit.BehaviorNodes
{
    public class MoveToEntity : Action
    {
        public SharedEntity Entity;

        public UnitMeshAgent UnitMeshAgent;

        private bool _finished;

        public override void OnStart()
        {
            _finished = false;

            UnitMeshAgent.DestinationReach += OnDestinationReach;
            UnitMeshAgent.SetDestinationToEntity(Entity.Value);
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
