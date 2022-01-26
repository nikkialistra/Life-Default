using System;
using Entities.Entity;
using NPBehave;
using Sirenix.OdinInspector;
using UnitManagement.Targeting;
using Units.Unit.BehaviorNodes;
using Units.Unit.UnitTypes;
using UnityEngine;

namespace Units.Unit
{
    [RequireComponent(typeof(UnitMeshAgent))]
    [RequireComponent(typeof(UnitClass))]
    public class UnitBehavior : MonoBehaviour
    {
        [MinValue(0)]
        [SerializeField] private float _seekRadius;

        [MinValue(0)]
        [SerializeField] private float _reactionSpeed;
        [MinValue(0)]
        [SerializeField] private float _viewRadius;


        private const string NewCommandKey = "newCommand";

        private const string PositionKey = "desiredPosition";
        private const string EntityKey = "entity";

        private const string UnitClassKey = "unitType";

        private Root _behaviorTree;

        private UnitMeshAgent _unitMeshAgent;
        private UnitClass _unitClass;

        private OrderMark _currentOrderMark;

        private bool _initialized;

        private void Awake()
        {
            _unitMeshAgent = GetComponent<UnitMeshAgent>();
            _unitClass = GetComponent<UnitClass>();
        }

        private void OnDestroy()
        {
            StopBehaviorTree();
        }

        public void Initialize()
        {
            if (_initialized)
            {
                return;
            }

            ConstructBehaviorTree();
            _initialized = true;

#if UNITY_EDITOR

            var debugger = (Debugger)gameObject.AddComponent(typeof(Debugger));
            debugger.BehaviorTree = _behaviorTree;
#endif
        }

        public void StartBehaviorTree()
        {
            if (!_initialized)
            {
                throw new InvalidOperationException("Cannot use behavior tree before initialization");
            }

            if (_behaviorTree is { CurrentState: Node.State.INACTIVE })
            {
                _behaviorTree.Start();
            }
        }

        public void StopBehaviorTree()
        {
            if (_behaviorTree is { CurrentState: Node.State.ACTIVE })
            {
                _behaviorTree.Stop();
            }
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
            if (!_unitMeshAgent.AcceptOrder())
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
            if (!_unitMeshAgent.AcceptOrder())
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
                        new ResetBehavior(NewCommandKey, _unitClass)
                    ),
                    new Selector(
                        new BlackboardCondition(PositionKey, Operator.IS_SET, Stops.NONE,
                            new MoveToPosition(PositionKey, _unitMeshAgent)),
                        new BlackboardCondition(EntityKey, Operator.IS_SET, Stops.NONE,
                            new Repeater(
                                new Sequence(
                                    new MoveToEntity(EntityKey, _unitMeshAgent),
                                    new RotateToEntity(EntityKey, _unitMeshAgent),
                                    new InteractWithEntity(EntityKey, _unitClass),
                                    new FindNewEntity(EntityKey, transform, _seekRadius)
                                )
                            )
                        )
                    ),
                    new Repeater(
                        new Sequence(
                            new Wait(_reactionSpeed),
                            new FindEnemy(EntityKey, transform, _viewRadius)
                        )
                    )
                )
            );
        }
    }
}
