using NPBehave;
using UnityEngine;

namespace Units.Unit.BehaviorNodes
{
    public class MoveToPosition : Task
    {
        private readonly UnitMeshAgent _unitMeshAgent;
        private readonly string _positionKey;

        public MoveToPosition(string positionKey, UnitMeshAgent unitMeshAgent) : base("MoveToPosition")
        {
            _positionKey = positionKey;
            _unitMeshAgent = unitMeshAgent;
        }

        protected override void DoStart()
        {
            if (!Blackboard.Isset(_positionKey))
            {
                Stopped(false);
                return;
            }

            var position = Blackboard.Get<Vector3>(_positionKey);
            Blackboard.Unset(_positionKey);

            _unitMeshAgent.DestinationReach += OnDestinationReach;
            _unitMeshAgent.SetDestinationToPosition(position);
        }

        private void OnDestinationReach()
        {
            _unitMeshAgent.DestinationReach -= OnDestinationReach;
            Stopped(true);
        }

        protected override void DoStop()
        {
            Debug.Log(2);
            _unitMeshAgent.DestinationReach -= OnDestinationReach;
            _unitMeshAgent.StopMoving();
            Stopped(false);
        }
    }
}
