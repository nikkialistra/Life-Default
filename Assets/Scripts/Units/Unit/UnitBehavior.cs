using System;
using NPBehave;
using UnitManagement.Targeting;
using Units.Unit.BehaviorNodes;
using Units.Unit.UnitTypes;
using UnityEngine;
using Action = NPBehave.Action;

namespace Units.Unit
{
    [RequireComponent(typeof(UnitMeshAgent))]
    public class UnitBehavior : MonoBehaviour, ITargetable
    {
        private const string PositionKey = "desiredPosition";
        private const string TargetMarkKey = "targetMark";
        private const string UnitTypeKey = "unitType";
        
        private Root _behaviorTree;
        
        private UnitMeshAgent _unitMeshAgent;

        private TargetMark _currentTargetMark;

        private void Awake()
        {
            _unitMeshAgent = GetComponent<UnitMeshAgent>();
        }

        public event Action<ITargetable> TargetReach;

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

        public void UpdateUnitType(UnitType unitType)
        {
            _behaviorTree.Blackboard.Set(UnitTypeKey, unitType);
        }

        public bool TryAcceptTargetWithPosition(TargetMark targetMark, Vector3 position)
        {
            if (!_unitMeshAgent.CanAcceptTargetPoint)
            {
                return false;
            }

            _behaviorTree.Blackboard.Set(TargetMarkKey, targetMark);
            _behaviorTree.Blackboard.Set(PositionKey, position);

            return true;
        }

        private void ConstructBehaviorTree()
        {
            _behaviorTree = new Root(
                new Selector(
                    new BlackboardCondition(PositionKey, Operator.IS_SET, Stops.BOTH,
                        new Sequence(
                            new MoveToPosition(_unitMeshAgent, PositionKey, OnTargetReach),
                            new CheckHasTarget(TargetMarkKey),
                            new Repeater(
                                new Sequence(
                                    new StartActionOnTarget(TargetMarkKey, UnitTypeKey),
                                    new FindNewTarget()
                                )
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

        private void OnTargetReach()
        {
            TargetReach?.Invoke(this);
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
