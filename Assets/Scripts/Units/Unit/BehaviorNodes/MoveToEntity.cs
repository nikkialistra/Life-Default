using Entities.Entity;
using NPBehave;

namespace Units.Unit.BehaviorNodes
{
    public class MoveToEntity : Node
    {
        private readonly string _entityKey;
        private readonly UnitMeshAgent _unitMeshAgent;

        public MoveToEntity(string entityKey, UnitMeshAgent unitMeshAgent) : base("MoveToEntity")
        {
            _entityKey = entityKey;
            _unitMeshAgent = unitMeshAgent;
        }

        protected override void DoStart()
        {
            if (!Blackboard.Isset(_entityKey))
            {
                Stopped(false);
                return;
            }

            var entity = Blackboard.Get<Entity>(_entityKey);

            _unitMeshAgent.DestinationReach += OnDestinationReach;
            _unitMeshAgent.SetDestinationToEntity(entity.transform.position);
        }

        private void OnDestinationReach()
        {
            _unitMeshAgent.DestinationReach -= OnDestinationReach;
            Stopped(true);
        }

        protected override void DoStop()
        {
            _unitMeshAgent.DestinationReach -= OnDestinationReach;
            _unitMeshAgent.StopMoving();
            Stopped(false);
        }
    }
}
