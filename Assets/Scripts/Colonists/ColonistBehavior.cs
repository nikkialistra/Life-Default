using System;
using System.Collections.Generic;
using BehaviorDesigner.Runtime;
using Entities;
using Entities.Types;
using Units.BehaviorVariables;
using UnityEngine;

namespace Colonists
{
    [RequireComponent(typeof(ColonistMeshAgent))]
    [RequireComponent(typeof(BehaviorTree))]
    public class ColonistBehavior : MonoBehaviour
    {
        private BehaviorTree _behaviorTree;

        private ColonistMeshAgent _colonistMeshAgent;

        private SharedPositions _positions;
        private SharedFloat _rotation;
        private SharedResource _resource;
        private SharedEnemy _enemy;

        private SharedBool _newCommand;

        private bool _initialized;

        private void Awake()
        {
            _colonistMeshAgent = GetComponent<ColonistMeshAgent>();
            _behaviorTree = GetComponent<BehaviorTree>();
        }

        public void Activate()
        {
            if (!_initialized)
            {
                Initialize();
            }

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

        public bool TryOrderToEntity(Entity entity)
        {
            if (!_colonistMeshAgent.AcceptOrder())
            {
                return false;
            }

            ResetParameters();
            SetParameterByType(entity);

            _newCommand.Value = true;

            return true;
        }

        private void SetParameterByType(Entity entity)
        {
            switch (entity.EntityType)
            {
                case EntityType.Colonist:
                    break;
                case EntityType.Enemy:
                    _enemy.Value = entity.Enemy;
                    break;
                case EntityType.Building:
                    break;
                case EntityType.Resource:
                    _resource.Value = entity.Resource;
                    break;
                default:
                    throw new ArgumentOutOfRangeException();
            }
        }

        private void ResetParameters()
        {
            _positions.Value.Clear();
            _rotation.Value = float.NegativeInfinity;
            _resource.Value = null;
            _enemy.Value = null;
        }

        public bool TryOrderToPosition(Vector3 position, float? angle)
        {
            if (!_colonistMeshAgent.AcceptOrder())
            {
                return false;
            }

            ResetParameters();
            _positions.Value.Enqueue(position);
            if (angle.HasValue)
            {
                _rotation.Value = angle.Value;
            }

            _newCommand.Value = true;

            return true;
        }

        public bool TryAddPositionToOrder(Vector3 position, float? angle)
        {
            if (_positions.Value.Count == 0)
            {
                return TryOrderToPosition(position, angle);
            }

            if (!_colonistMeshAgent.AcceptOrder())
            {
                return false;
            }

            _positions.Value.Enqueue(position);
            if (angle.HasValue)
            {
                _rotation.Value = angle.Value;
            }

            return true;
        }

        private void Initialize()
        {
            _newCommand = (SharedBool)_behaviorTree.GetVariable("NewCommand");

            _positions = (SharedPositions)_behaviorTree.GetVariable("Positions");
            _rotation = (SharedFloat)_behaviorTree.GetVariable("Rotation");
            _resource = (SharedResource)_behaviorTree.GetVariable("Resource");
            _enemy = (SharedEnemy)_behaviorTree.GetVariable("Enemy");

            _positions.Value = new Queue<Vector3>();

            _initialized = true;
        }
    }
}
