using System;
using Enemies.Enemy.BehaviorNodes;
using NPBehave;
using Sirenix.OdinInspector;
using UnityEngine;

namespace Enemies.Enemy
{
    [RequireComponent(typeof(EnemyMeshAgent))]
    public class EnemyBehavior : MonoBehaviour
    {
        [MinValue(0)]
        [SerializeField] private float _walkMinRadius;
        [MinValue(0)]
        [SerializeField] private float _walkMaxRadius;
        [MinValue(0)]
        [SerializeField] private float _waitTime = 3;
        [MinValue(0)]
        [SerializeField] private float _waitVariance = 1.5f;

        private Root _behaviorTree;

        private EnemyMeshAgent _enemyMeshAgent;

        private bool _initialized;

        private void Awake()
        {
            _enemyMeshAgent = GetComponent<EnemyMeshAgent>();
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

        private void ConstructBehaviorTree()
        {
            _behaviorTree = new Root(
                new Repeater(
                    new Sequence(
                        new Wait(_waitTime, _waitVariance),
                        new WalkToRandomLocation(_enemyMeshAgent, _walkMinRadius, _walkMaxRadius)
                    )
                )
            );
        }
    }
}
