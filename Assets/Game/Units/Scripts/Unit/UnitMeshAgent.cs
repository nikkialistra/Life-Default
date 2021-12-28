using System;
using Kernel.Types;
using Sirenix.OdinInspector;
using UnityEngine;
using UnityEngine.AI;
using Random = UnityEngine.Random;

namespace Game.Units.Unit
{
    [RequireComponent(typeof(UnitFacade))]
    [RequireComponent(typeof(NavMeshAgent))]
    public class UnitMeshAgent : MonoBehaviour, ITargetable
    {
        [MinValue(0)]
        [SerializeField] private float _distanceToGroup;

        private UnitFacade _unitFacade;
        private NavMeshAgent _navMeshAgent;

        private void Awake()
        {
            _unitFacade = GetComponent<UnitFacade>();
            _navMeshAgent = GetComponent<NavMeshAgent>();
        }

        private void OnEnable()
        {
            _unitFacade.Die += Stop;
        }

        private void OnDisable()
        {
            _unitFacade.Die -= Stop;
        }

        public bool TryAcceptPoint(GameObject point)
        {
            var destinationSet = _navMeshAgent.SetDestination(point.transform.position + Random.insideUnitSphere * _distanceToGroup);
            return destinationSet;
        }

        private void Stop()
        {
            _navMeshAgent.ResetPath();
        }
    }
}