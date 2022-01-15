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
        private Root _behaviorTree;
        
        private UnitMeshAgent _unitMeshAgent;

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

            _unitMeshAgent.SetDestination(position);
            
            return true;
        }

        private void ConstructBehaviorTree()
        {
            _behaviorTree = new Root(
                new Sequence(
                    new Action(() => Debug.Log("Hello World!")),
                    new WaitUntilStopped()
                )
            );
        }

        private void OnMeshAgentTargetReach()
        {
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
