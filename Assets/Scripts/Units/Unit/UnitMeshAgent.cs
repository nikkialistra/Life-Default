using System;
using System.Collections;
using Pathfinding;
using UnityEngine;

namespace Units.Unit
{
    [RequireComponent(typeof(UnitFacade))]
    [RequireComponent(typeof(AIPath))]
    public class UnitMeshAgent : MonoBehaviour
    {
        private bool _activated;

        private Coroutine _movingCoroutine;

        private UnitFacade _unitFacade;
        private AIPath _aiPath;

        private void Awake()
        {
            _unitFacade = GetComponent<UnitFacade>();
            _aiPath = GetComponent<AIPath>();
        }

        public event Action TargetReach;
        
        public float Velocity => _aiPath.velocity.magnitude;

        public bool CanAcceptTargetPoint => _activated;

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
            
            TargetReach?.Invoke();
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
