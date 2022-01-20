using Entities.Entity;
using NPBehave;

namespace Units.Unit.BehaviorNodes
{
    public class RotateToEntity : Node
    {
        private readonly string _entityKey;
        private readonly UnitMeshAgent _unitMeshAgent;

        public RotateToEntity(string entityKey, UnitMeshAgent unitMeshAgent) : base("RotateToEntity")
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

            _unitMeshAgent.RotationEnd += OnRotationEnd;
            _unitMeshAgent.RotateTo(entity);
        }

        private void OnRotationEnd()
        {
            _unitMeshAgent.RotationEnd -= OnRotationEnd;
            Stopped(true);
        }

        protected override void DoStop()
        {
            _unitMeshAgent.RotationEnd -= OnRotationEnd;
            _unitMeshAgent.StopRotating();
            Stopped(false);
        }
    }
}
