using System;
using NPBehave;
using UnitManagement.Targeting;
using UnityEngine;
using Action = System.Action;

namespace Units.Unit.BehaviorNodes
{
    public class MoveToPosition : Task
    {
        private readonly UnitMeshAgent _unitMeshAgent;
        private readonly string _desiredPositionKey;
        private readonly Action _callback;

        public MoveToPosition(UnitMeshAgent unitMeshAgent, string desiredPositionKey, Action callback) : base("MoveToPosition")
        {
            _callback = callback;
            _desiredPositionKey = desiredPositionKey;
            _unitMeshAgent = unitMeshAgent;
        }

        protected override void DoStart()
        {
            var desiredPosition = Blackboard.Get<Vector3>(_desiredPositionKey);
            Blackboard.Unset(_desiredPositionKey);
            
            _unitMeshAgent.SetDestination(desiredPosition);
            _unitMeshAgent.TargetReach += OnTargetReach;
        }

        protected override void DoStop()
        {
            Stopped(false);
        }

        private void OnTargetReach()
        {
            _unitMeshAgent.TargetReach -= OnTargetReach;
            _callback();
            Stopped(true);
        }
    }
}
