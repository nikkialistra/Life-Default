using BehaviorDesigner.Runtime;
using Entities.Entity;
using Units.Unit.UnitTypes;
using UnityEngine;

namespace Units.Unit
{
    [RequireComponent(typeof(UnitMeshAgent))]
    [RequireComponent(typeof(UnitClass))]
    [RequireComponent(typeof(BehaviorTree))]
    public class UnitBehavior : MonoBehaviour
    {
        private BehaviorTree _behaviorTree;

        private UnitMeshAgent _unitMeshAgent;

        private SharedBool _newCommand;
        private SharedVector3 _position;
        private SharedEntity _entity;

        private void Awake()
        {
            _unitMeshAgent = GetComponent<UnitMeshAgent>();
            _behaviorTree = GetComponent<BehaviorTree>();
        }

        private void Start()
        {
            _newCommand = (SharedBool)_behaviorTree.GetVariable("NewCommand");
            _position = (SharedVector3)_behaviorTree.GetVariable("Position");
            _entity = (SharedEntity)_behaviorTree.GetVariable("Entity");
        }

        public void Enable()
        {
            _behaviorTree.EnableBehavior();
        }

        public void Disable()
        {
            _behaviorTree.DisableBehavior();
        }

        public void Stop()
        {
            _entity.Value = null;
            _position.Value = Vector3.negativeInfinity;

            _newCommand.Value = true;
        }

        public bool TryOrderToEntity(Entity entity)
        {
            if (!_unitMeshAgent.AcceptOrder())
            {
                return false;
            }

            _entity.Value = entity;
            _position.Value = Vector3.negativeInfinity;

            _newCommand.Value = true;

            return true;
        }

        public bool TryOrderToPosition(Vector3 position)
        {
            if (!_unitMeshAgent.AcceptOrder())
            {
                return false;
            }

            _entity.Value = null;
            _position.Value = position;

            _newCommand.Value = true;

            return true;
        }
    }
}
