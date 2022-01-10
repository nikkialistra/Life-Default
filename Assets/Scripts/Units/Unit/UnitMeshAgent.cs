using System;
using System.Collections;
using Pathfinding;
using Sirenix.OdinInspector;
using UnitManagement.Targeting;
using UnityEngine;
using Random = UnityEngine.Random;

namespace Units.Unit
{
    [RequireComponent(typeof(UnitFacade))]
    [RequireComponent(typeof(AIPath))]
    public class UnitMeshAgent : MonoBehaviour, ITargetable
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

        public event Action<ITargetable> TargetReach;

        public GameObject GameObject => gameObject;
        public float Velocity => _aiPath.velocity.magnitude;

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

        private void Activate()
        {
            _aiPath.isStopped = false;
            _activated = true;
        }

        public bool AcceptTarget(Target target)
        {
            if (!_activated)
            {
                return false;
            }

            _aiPath.destination = target.transform.position;
            _aiPath.OnTargetReached();
            Move();
            return true;
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

            TargetReach?.Invoke(this);
        }

        private bool IsMoving()
        {
            return !_aiPath.reachedDestination;
        }

        private void Deactivate()
        {
            _aiPath.isStopped = true;
            _activated = false;
        }
    }
}