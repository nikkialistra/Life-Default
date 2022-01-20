using Entities.Entity;
using NPBehave;
using Sirenix.OdinInspector;
using UnitManagement.Targeting;
using Units.Unit.BehaviorNodes;
using Units.Unit.UnitType;
using UnityEngine;

namespace Units.Unit
{
    [RequireComponent(typeof(UnitMeshAgent))]
    public class UnitBehavior : MonoBehaviour
    {
        [MinValue(0)]
        [SerializeField] private float _seekRadius;

        private const string NewCommandKey = "newCommand";

        private const string PositionKey = "desiredPosition";
        private const string EntityKey = "entity";

        private const string UnitClassKey = "unitType";

        private Root _behaviorTree;

        private UnitMeshAgent _unitMeshAgent;

        private OrderMark _currentOrderMark;

        private void Awake()
        {
            _unitMeshAgent = GetComponent<UnitMeshAgent>();
        }

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

        public void Stop()
        {
            _behaviorTree.Blackboard.Unset(PositionKey);
            _behaviorTree.Blackboard.Unset(EntityKey);

            _behaviorTree.Blackboard.Set(NewCommandKey, true);
        }

        public bool TryOrderToEntity(Entity entity)
        {
            if (!_unitMeshAgent.CanAcceptOrder)
            {
                return false;
            }

            _behaviorTree.Blackboard.Unset(PositionKey);

            _behaviorTree.Blackboard.Set(EntityKey, entity);
            _behaviorTree.Blackboard.Set(NewCommandKey, true);

            return true;
        }

        public bool TryOrderToPosition(Vector3 position)
        {
            if (!_unitMeshAgent.CanAcceptOrder)
            {
                return false;
            }

            _behaviorTree.Blackboard.Unset(EntityKey);

            _behaviorTree.Blackboard.Set(PositionKey, position);
            _behaviorTree.Blackboard.Set(NewCommandKey, true);

            return true;
        }

        private void ConstructBehaviorTree()
        {
            _behaviorTree = new Root(
                new Selector(
                    new BlackboardCondition(NewCommandKey, Operator.IS_SET, Stops.LOWER_PRIORITY,
                        new ResetBehavior(NewCommandKey, UnitClassKey)
                    ),
                    new Selector(
                        new BlackboardCondition(PositionKey, Operator.IS_SET, Stops.NONE,
                            new MoveToPosition(_unitMeshAgent, PositionKey)),
                        new BlackboardCondition(EntityKey, Operator.IS_SET, Stops.NONE,
                            new Repeater(
                                new Sequence(
                                    new MoveToEntity(EntityKey, _unitMeshAgent),
                                    new RotateToEntity(EntityKey, _unitMeshAgent),
                                    new InteractWithEntity(EntityKey, UnitClassKey),
                                    new FindNewEntity(EntityKey, transform, _seekRadius)
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

        private void ShowIdleState()
        {
            Debug.Log("Idle");
        }

        private void StopBehaviorTree()
        {
            if (_behaviorTree is { CurrentState: Node.State.ACTIVE })
            {
                _behaviorTree.Stop();
            }
        }
    }
}
