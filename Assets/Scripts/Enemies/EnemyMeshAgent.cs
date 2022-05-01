using System;
using Units;
using UnityEngine;

namespace Enemies
{
    [RequireComponent(typeof(UnitMeshAgent))]
    [RequireComponent(typeof(EnemyAnimator))]
    public class EnemyMeshAgent : MonoBehaviour
    {
        private UnitMeshAgent _unitMeshAgent;
        private EnemyAnimator _animator;

        private void Awake()
        {
            _unitMeshAgent = GetComponent<UnitMeshAgent>();
            _animator = GetComponent<EnemyAnimator>();
        }
        
        public bool Idle { get; private set; } = true;

        private void OnEnable()
        {
            _unitMeshAgent.DestinationReach += OnDestinationReach;
        }

        private void OnDisable()
        {
            _unitMeshAgent.DestinationReach -= OnDestinationReach;
        }

        public void Deactivate()
        {
            _unitMeshAgent.StopCurrentCommand();
        }

        public void GoToPosition(Vector3 position)
        {
            Idle = false;
            _unitMeshAgent.SetDestinationToPosition(position);
            _animator.Move(true);
        }

        public void StopMoving()
        {
            _unitMeshAgent.StopMoving();
            _animator.Move(false);
        }

        private void OnDestinationReach()
        {
            Idle = true;
            _animator.Move(false);
        }
    }
}
