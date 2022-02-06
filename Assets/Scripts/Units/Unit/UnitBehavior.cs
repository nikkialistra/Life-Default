using System;
using BehaviorDesigner.Runtime;
using Entities;
using Entities.BehaviorVariables;
using Entities.Types;
using Units.Unit.UnitTypes;
using UnityEngine;

namespace Units.Unit
{
    [RequireComponent(typeof(UnitMeshAgent))]
    [RequireComponent(typeof(UnitRole))]
    [RequireComponent(typeof(BehaviorTree))]
    public class UnitBehavior : MonoBehaviour
    {
        private BehaviorTree _behaviorTree;

        private UnitMeshAgent _unitMeshAgent;

        private SharedVector3 _position;
        private SharedResource _resource;
        private SharedEnemy _enemy;

        private SharedBool _newCommand;

        private bool _initialized;

        private void Awake()
        {
            _unitMeshAgent = GetComponent<UnitMeshAgent>();
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
            if (!_unitMeshAgent.AcceptOrder())
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
                case EntityType.Unit:
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
            _position.Value = Vector3.negativeInfinity;
            _resource.Value = null;
            _enemy.Value = null;
        }

        public bool TryOrderToPosition(Vector3 position)
        {
            if (!_unitMeshAgent.AcceptOrder())
            {
                return false;
            }

            ResetParameters();
            _position.Value = position;

            _newCommand.Value = true;

            return true;
        }

        private void Initialize()
        {
            _newCommand = (SharedBool)_behaviorTree.GetVariable("NewCommand");

            _position = (SharedVector3)_behaviorTree.GetVariable("Position");
            _resource = (SharedResource)_behaviorTree.GetVariable("Resource");
            _enemy = (SharedEnemy)_behaviorTree.GetVariable("Enemy");

            _position.Value = Vector3.negativeInfinity;

            _initialized = true;
        }
    }
}
