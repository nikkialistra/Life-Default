using BehaviorDesigner.Runtime.Tasks;
using Entities.Entity;
using Units.Unit.UnitTypes;

namespace Units.Unit.BehaviorNodes
{
    public class MoveToResource : Action
    {
        public SharedResource Resource;

        public UnitMeshAgent UnitMeshAgent;
        public UnitRole UnitRole;

        private bool _finished;

        public override void OnStart()
        {
            _finished = false;

            UnitMeshAgent.DestinationReach += OnDestinationReach;
            UnitMeshAgent.SetDestinationToEntity(Resource.Value.Entity,
                UnitRole.GetInteractionDistanceWith(Resource.Value));
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
