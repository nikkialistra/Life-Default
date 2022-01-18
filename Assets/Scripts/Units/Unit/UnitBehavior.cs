using System;
using NPBehave;
using UnitManagement.Targeting;
using Units.Unit.BehaviorNodes;
using Units.Unit.UnitType;
using UnityEngine;
using Action = NPBehave.Action;

namespace Units.Unit
{
    [RequireComponent(typeof(UnitMeshAgent))]
    public class UnitBehavior : MonoBehaviour, IOrderable
    {
        private const string PositionKey = "desiredPosition";
        private const string TargetKey = "target";
        private const string NewCommandKey = "newCommand";
        private const string UnitClassKey = "unitType";
        
        private Root _behaviorTree;
        
        private UnitMeshAgent _unitMeshAgent;

        private OrderMark _currentOrderMark;

        private void Awake()
        {
            _unitMeshAgent = GetComponent<UnitMeshAgent>();
        }

        public event Action<IOrderable> DestinationReach;

        public GameObject GameObject => gameObject;

        public Vector3 Position => transform.position;

        private void OnDestroy()
        {
            StopBehaviorTree();
        }

        public void Initialize()
        {
            ConstructBehaviorTree();

#if UNITY_EDITOR
            
            var debugger = (Debugger)gameObject.AddComponent(typeof(Debugger));
            debugger.BehaviorTree = _behaviorTree;
#endif
            
            _behaviorTree.Start();
        }

        public void ChangeUnitClass(UnitClass unitClass)
        {
            _behaviorTree.Blackboard.Set(UnitClassKey, unitClass);
        }

        public bool TryOrderToTargetWithPosition(Target target, Vector3 position)
        {
            if (!_unitMeshAgent.CanAcceptOrder)
            {
                return false;
            }

            _behaviorTree.Blackboard.Set(TargetKey, target);
            _behaviorTree.Blackboard.Set(PositionKey, position);
            
            _behaviorTree.Blackboard.Set(NewCommandKey, true);

            return true;
        }

        public bool TryOrderToPosition(Vector3 position)
        {
            if (!_unitMeshAgent.CanAcceptOrder)
            {
                return false;
            }
            
            _behaviorTree.Blackboard.Set(PositionKey, position);
            
            _behaviorTree.Blackboard.Set(NewCommandKey, true);

            return true;
        }

        private void ConstructBehaviorTree()
        {
            _behaviorTree = new Root(
                new Selector(
                    new BlackboardCondition(NewCommandKey, Operator.IS_SET, Stops.LOWER_PRIORITY,
                        new ClearCommand(NewCommandKey)
                    ),
                    new Sequence(
                        new MoveToPosition(_unitMeshAgent, PositionKey, OnDestinationReach),
                        new CheckHasTarget(TargetKey),
                        new Repeater(
                            new Sequence(
                                new StartActionOnTarget(TargetKey, UnitClassKey),
                                new FindNewTarget()
                            )
                        )
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

        private void OnDestinationReach()
        {
            DestinationReach?.Invoke(this);
        }

        private void ShowIdleState()
        {
            Debug.Log("Idle");
        }
        
        private void ShowIdleState2()
        {
            Debug.Log("Idle2");
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
