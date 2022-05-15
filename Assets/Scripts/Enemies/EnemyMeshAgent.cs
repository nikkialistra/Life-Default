using System;
using Units;
using UnityEngine;
using Random = UnityEngine.Random;

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
        
        private void Update()
        {
            if (_unitMeshAgent.IsMoving)
            {
                _animator.Move();
            }
            else
            {
                _animator.Idle();
            }
        }

        public void Deactivate()
        {
            _unitMeshAgent.StopCurrentCommand();
        }

        public void GoToPosition(Vector3 position)
        {
            Idle = false;
            _unitMeshAgent.SetDestinationToPosition(position);
            _animator.Move();
        }

        public void RunFrom(Unit opponent, float distance, float randomizationRadius)
        {
            var delta = transform.position - opponent.transform.position;
            var flatNormalizedDelta = new Vector3(delta.x, 0, delta.z).normalized;
            var randomizedDelta = Random.insideUnitSphere * randomizationRadius;
            
            var oppositePoint = transform.position + (flatNormalizedDelta * distance) + new Vector3(randomizedDelta.x, 0, randomizedDelta.z);

            GoToPosition(oppositePoint);
        }

        public void StopMoving()
        {
            _unitMeshAgent.StopMoving();
            _animator.Idle();
        }

        public void StopRotating()
        {
            _unitMeshAgent.StopRotating();
        }

        private void OnDestinationReach()
        {
            Idle = true;
            _animator.Idle();
        }
    }
}
