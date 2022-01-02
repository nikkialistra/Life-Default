using System;
using System.Collections;
using Sirenix.OdinInspector;
using UnitManagement.Targeting;
using UnityEngine;
using UnityEngine.AI;
using Random = UnityEngine.Random;

namespace Units.Unit
{
    [RequireComponent(typeof(UnitFacade))]
    [RequireComponent(typeof(NavMeshAgent))]
    public class UnitMeshAgent : MonoBehaviour, ITargetable
    {
        [MinValue(0)]
        [SerializeField] private float _distanceToGroup;

        public event Action<ITargetable> TargetReach;

        public GameObject GameObject => gameObject;

        private bool _activated;

        private Coroutine _movingCoroutine;

        private UnitFacade _unitFacade;
        private NavMeshAgent _navMeshAgent;


        private void Awake()
        {
            _unitFacade = GetComponent<UnitFacade>();
            _navMeshAgent = GetComponent<NavMeshAgent>();
        }

        private void OnEnable()
        {
            _navMeshAgent.enabled = false;
            _unitFacade.Spawn += ActivateSelf;
            _unitFacade.Die += Deactivate;
        }

        private void OnDisable()
        {
            _unitFacade.Spawn -= ActivateSelf;
            _unitFacade.Die -= Deactivate;
        }

        private void ActivateSelf()
        {
            _activated = true;
            _navMeshAgent.enabled = true;
        }

        public bool TryAcceptTarget(Target target)
        {
            if (!_activated)
            {
                return false;
            }

            var destinationSet =
                _navMeshAgent.SetDestination(target.transform.position + Random.insideUnitSphere * _distanceToGroup);
            if (destinationSet)
            {
                Move();
            }

            return destinationSet;
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
            while (NavMeshAgentWorking())
            {
                yield return null;
            }

            TargetReach?.Invoke(this);
        }

        private bool NavMeshAgentWorking()
        {
            return _navMeshAgent.pathPending || _navMeshAgent.remainingDistance > _navMeshAgent.stoppingDistance;
        }

        private void Deactivate()
        {
            _navMeshAgent.ResetPath();
            _activated = false;
        }
    }
}