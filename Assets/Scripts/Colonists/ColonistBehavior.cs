using System.Collections.Generic;
using BehaviorDesigner.Runtime;
using ResourceManagement;
using Units;
using Units.BehaviorVariables;
using UnityEngine;

namespace Colonists
{
    [RequireComponent(typeof(BehaviorTree))]
    [RequireComponent(typeof(ColonistMeshAgent))]
    public class ColonistBehavior : MonoBehaviour
    {
        private BehaviorTree _behaviorTree;

        private ColonistMeshAgent _meshAgent;

        private SharedBool _newCommand;

        private SharedPositions _positions;
        private SharedFloat _rotation;
        private SharedColonist _colonist;
        private SharedEnemy _enemy;
        private SharedResource _resource;
        private SharedUnit _unitTarget;

        private bool _initialized;

        private void Awake()
        {
            _behaviorTree = GetComponent<BehaviorTree>();

            _meshAgent = GetComponent<ColonistMeshAgent>();
        }

        public void Activate()
        {
            if (!_initialized)
                Initialize();

            _behaviorTree.EnableBehavior();
        }

        public void Deactivate()
        {
            _behaviorTree.DisableBehavior();
        }

        public void Stop()
        {
            ResetParameters();
            _newCommand.Value = true;
        }

        public void OrderTo(Colonist targetColonist)
        {
            if (_meshAgent.CanAcceptOrder() == false)
                return;

            ResetParameters();
            _colonist.Value = targetColonist;

            _newCommand.Value = true;
        }

        public void OrderTo(Unit unitTarget)
        {
            if (!_meshAgent.CanAcceptOrder()) return;

            ResetParameters();
            _unitTarget.Value = unitTarget;

            _newCommand.Value = true;
        }

        public void OrderTo(Resource resource)
        {
            if (!_meshAgent.CanAcceptOrder()) return;

            ResetParameters();
            _resource.Value = resource;

            _newCommand.Value = true;
        }

        private void ResetParameters()
        {
            _positions.Value.Clear();
            _rotation.Value = float.NegativeInfinity;

            _colonist.Value = null;
            _unitTarget.Value = null;
            _resource.Value = null;
        }

        public void OrderToPosition(Vector3 position, float? angle)
        {
            if (!_meshAgent.CanAcceptOrder()) return;

            ResetParameters();
            _positions.Value.Enqueue(position);

            if (angle.HasValue)
                _rotation.Value = angle.Value;

            _newCommand.Value = true;
        }

        public void AddPositionToOrder(Vector3 position, float? angle)
        {
            if (!_meshAgent.CanAcceptOrder()) return;

            if (_positions.Value.Count == 0)
                OrderToPosition(position, angle);

            _positions.Value.Enqueue(position);

            if (angle.HasValue)
                _rotation.Value = angle.Value;
        }

        private void Initialize()
        {
            _newCommand = (SharedBool)_behaviorTree.GetVariable("NewCommand");

            _positions = (SharedPositions)_behaviorTree.GetVariable("Positions");
            _rotation = (SharedFloat)_behaviorTree.GetVariable("Rotation");

            _colonist = (SharedColonist)_behaviorTree.GetVariable("Colonist");
            _unitTarget = (SharedUnit)_behaviorTree.GetVariable("UnitTarget");
            _resource = (SharedResource)_behaviorTree.GetVariable("Resource");

            _positions.Value = new Queue<Vector3>();

            _initialized = true;
        }
    }
}
