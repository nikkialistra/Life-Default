using System;
using System.Collections;
using Entities.Entity;
using Pathfinding;
using UnityEngine;

namespace Units.Unit
{
    [RequireComponent(typeof(UnitFacade))]
    [RequireComponent(typeof(AIPath))]
    public class UnitMeshAgent : MonoBehaviour
    {
        [SerializeField] private float _rotationSpeedToEntities;
        
        private bool _activated;

        private Coroutine _movingCoroutine;

        private UnitFacade _unitFacade;
        private AIPath _aiPath;

        private void Awake()
        {
            _unitFacade = GetComponent<UnitFacade>();
            _aiPath = GetComponent<AIPath>();
        }

        public event Action DestinationReach;
        public event Action RotationEnd;
        
        public float Velocity => _aiPath.velocity.magnitude;

        public bool CanAcceptOrder => _activated;

        private void OnEnable()
        {
            _unitFacade.Spawn += Activate;
            _unitFacade.Die += Deactivate;
        }

        private void OnDisable()
        {
            _unitFacade.Spawn -= Activate;
            _unitFacade.Die -= Deactivate;
        }

        public void SetDestination(Vector3 position)
        {
            _aiPath.isStopped = false;
            _aiPath.destination = position;
            Move();
        }

        public void RotateTo(Entity entity)
        {
            StartCoroutine(RotatingTo(entity));
        }

        private IEnumerator RotatingTo(Entity entity)
        {
            var targetDirection = (entity.transform.position - transform.position).normalized;
            var targetRotation = Quaternion.LookRotation(targetDirection);

            while (!IsApproximate(targetRotation, transform.rotation))
            {
                transform.rotation = Quaternion.Slerp(transform.rotation, targetRotation, Time.fixedDeltaTime * _rotationSpeedToEntities);
                
                yield return new WaitForFixedUpdate();
            }
            
            RotationEnd?.Invoke();
        }

        // Is the difference below 1 degree on 1 axis
        private static bool IsApproximate(Quaternion first, Quaternion second)
        {
            return Mathf.Abs(Quaternion.Dot(first, second)) >= 1 - 0.0000004f;
        }

        private void Activate()
        {
            _aiPath.isStopped = false;
            _activated = true;
        }

        private void Move()
        {
            if (_movingCoroutine != null)
            {
                StopCoroutine(_movingCoroutine);
            }

            _movingCoroutine = StartCoroutine(Moving());
        }

        private IEnumerator Moving()
        {
            while (IsMoving())
            {
                yield return null;
            }
            
            DestinationReach?.Invoke();
        }

        private bool IsMoving()
        {
            if (_aiPath.reachedDestination)
            {
                _aiPath.isStopped = true;
            }
            
            return !_aiPath.reachedDestination;
        }

        private void Deactivate()
        {
            _aiPath.isStopped = true;
            _activated = false;
        }
    }
}
