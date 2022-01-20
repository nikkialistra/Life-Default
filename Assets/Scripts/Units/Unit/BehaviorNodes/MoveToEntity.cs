using Entities.Entity;
using NPBehave;
using Action = System.Action;

namespace Units.Unit.BehaviorNodes
{
    public class MoveToEntity : Node
    {
        private readonly string _entityKey;
        private readonly UnitMeshAgent _unitMeshAgent;
        private readonly Action _callback;

        public MoveToEntity(string entityKey, UnitMeshAgent unitMeshAgent, Action callback) : base("MoveToEntity")
        {
            _entityKey = entityKey;
            _unitMeshAgent = unitMeshAgent;
            _callback = callback;
        }

        protected override void DoStart()
        {
            if (!Blackboard.Isset(_entityKey))
            {
                Stopped(false);
                return;
            }

            var entity = Blackboard.Get<Entity>(_entityKey);

            _unitMeshAgent.SetDestinationToEntity(entity.transform.position);
            _unitMeshAgent.DestinationReach += OnDestinationReach;
        }

        protected override void DoStop()
        {
            _unitMeshAgent.DestinationReach -= OnDestinationReach;
            Stopped(false);
        }

        private void OnDestinationReach()
        {
            _unitMeshAgent.DestinationReach -= OnDestinationReach;
            _callback();
            Stopped(true);
        }
    }
}
