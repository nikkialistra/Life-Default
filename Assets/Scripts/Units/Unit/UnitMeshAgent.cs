using Sirenix.OdinInspector;
using UnitManagement.Targeting;
using UnityEngine;
using UnityEngine.AI;

namespace Units.Unit
{
    [RequireComponent(typeof(UnitFacade))]
    [RequireComponent(typeof(NavMeshAgent))]
    public class UnitMeshAgent : MonoBehaviour, ITargetable
    {
        [MinValue(0)]
        [SerializeField] private float _distanceToGroup;

        private bool _activated;
        
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

        public bool TryAcceptPoint(GameObject point)
        {
            if (!_activated)
            {
                return false;
            }
            
            var destinationSet = _navMeshAgent.SetDestination(point.transform.position + Random.insideUnitSphere * _distanceToGroup);
            return destinationSet;
        }

        private void Deactivate()
        {
            _navMeshAgent.ResetPath();
            _activated = false;
        }
    }
}