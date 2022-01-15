using System;
using NPBehave;
using UnitManagement.Targeting;
using UnityEngine;
using Action = NPBehave.Action;

namespace Units.Unit
{
    [RequireComponent(typeof(UnitMeshAgent))]
    public class UnitBehavior : MonoBehaviour, ITargetable
    {
        private const string MovingToTarget = "movingToTarget";
        
        private Root _behaviorTree;
        
        private UnitMeshAgent _unitMeshAgent;
        
        private Vector3 _desiredPosition;

        private void Awake()
        {
            _unitMeshAgent = GetComponent<UnitMeshAgent>();
        }

        public event Action<ITargetable> TargetReach;

        public GameObject GameObject => gameObject;

        public Vector3 Position => transform.position;

        private void OnEnable()
        {
            _unitMeshAgent.TargetReach += OnMeshAgentTargetReach;
        }

        private void OnDisable()
        {
            _unitMeshAgent.TargetReach -= OnMeshAgentTargetReach;
        }

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

        public bool TryAcceptTargetPoint(Vector3 position)
        {
            if (!_unitMeshAgent.CanAcceptTargetPoint)
            {
                return false;
            }

            _desiredPosition = position;
            _behaviorTree.Blackboard.Set(MovingToTarget, true);

            return true;
        }

        private void ConstructBehaviorTree()
        {
            _behaviorTree = new Root(
                new Selector(
                    new BlackboardCondition(MovingToTarget, Operator.IS_SET, Stops.LOWER_PRIORITY,
                        new Action(MoveToPosition)
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

        private void MoveToPosition()
        {
            _unitMeshAgent.SetDestination(_desiredPosition);
        }

        private void ShowIdleState()
        {
            Debug.Log("Hello world");
        }

        private void OnMeshAgentTargetReach()
        {
            _behaviorTree.Blackboard.Unset(MovingToTarget);
            TargetReach?.Invoke(this);
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
