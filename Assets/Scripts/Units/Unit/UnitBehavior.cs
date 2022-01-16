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
        private const string PositionKey = "desiredPosition";
        private const string TargetKey = "target";
        
        private Root _behaviorTree;
        
        private UnitMeshAgent _unitMeshAgent;

        private Target _currentTarget;

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

            _behaviorTree.Blackboard.Set(TargetKey, target);
            _behaviorTree.Blackboard.Set(PositionKey, position);

            return true;
        }

        private void ConstructBehaviorTree()
        {
            _behaviorTree = new Root(
                new Selector(
                    new BlackboardCondition(PositionKey, Operator.IS_SET, Stops.LOWER_PRIORITY,
                        new Sequence(
                            new MoveToPosition(_unitMeshAgent, PositionKey, OnTargetReach),
                            new CheckHasTargetObject(TargetKey),
                            new Repeater(
                                new Sequence(
                                    new StartActionOnTargetObject(TargetKey),
                                    new FindNewTargetObject()
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
