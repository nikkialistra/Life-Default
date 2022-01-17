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
        private readonly string _positionKey;
        private readonly Action _callback;

        public MoveToPosition(UnitMeshAgent unitMeshAgent, string positionKey, Action callback) : base("MoveToPosition")
        {
            _callback = callback;
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
            
            _unitMeshAgent.SetDestination(position);
            _unitMeshAgent.TargetReach += OnTargetReach;
        }

        protected override void DoStop()
        {
            _unitMeshAgent.TargetReach -= OnTargetReach;
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
