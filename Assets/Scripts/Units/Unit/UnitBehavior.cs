using System;
using NPBehave;
using UnitManagement.Targeting;
using Units.Unit.BehaviorNodes;
using UnityEngine;
using Action = NPBehave.Action;

namespace Units.Unit
{
    [RequireComponent(typeof(UnitMeshAgent))]
    public class UnitBehavior : MonoBehaviour, ITargetable
    {
        private const string DesiredPosition = "desiredPosition";
        
        private Root _behaviorTree;
        
        private UnitMeshAgent _unitMeshAgent;

        private Target _currentTarget;
        private Vector3 _desiredPosition;

        private void Awake()
        {
            _unitMeshAgent = GetComponent<UnitMeshAgent>();
        }

        public event Action<ITargetable> TargetReach;

        public GameObject GameObject => gameObject;

        public Vector3 Position => transform.position;

        private void Start()
        {
            ConstructBehaviorTree();

#if UNITY_EDITOR
            
            var debugger = (Debugger)gameObject.AddComponent(typeof(Debugger));
            debugger.BehaviorTree = _behaviorTree;
#endif
            
            _behaviorTree.Start();
        }
        
        public void OnDestroy()
        {
            StopBehaviorTree();
        }

        public bool TryAcceptTargetWithPosition(Target target, Vector3 position)
        {
            if (!_unitMeshAgent.CanAcceptTargetPoint)
            {
                return false;
            }
            
            _currentTarget = target;
            _desiredPosition = position;
            
            _behaviorTree.Blackboard.Set(DesiredPosition, _desiredPosition);

            return true;
        }

        private void ConstructBehaviorTree()
        {
            _behaviorTree = new Root(
                new Selector(
                    new BlackboardCondition(DesiredPosition, Operator.IS_SET, Stops.LOWER_PRIORITY,
                        new MoveToPosition(_unitMeshAgent, DesiredPosition, OnTargetReach)
                    ),
                    new Repeater(
                        new Sequence(
                            new Wait(1.0f),
                            new Action(ShowIdleState)
                        )
                    )
                )
            );
        }

        private void OnTargetReach()
        {
            TargetReach?.Invoke(this);
        }

        private void ShowIdleState()
        {
            Debug.Log("Idle");
        }

        private void StopBehaviorTree()
        {
            if ( _behaviorTree is { CurrentState: Node.State.ACTIVE } )
            {
                _behaviorTree.Stop();
            }
        }
    }
}
