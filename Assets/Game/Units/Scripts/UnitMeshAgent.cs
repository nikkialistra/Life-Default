using Kernel.Types;
using UnityEngine;
using UnityEngine.AI;
using Zenject;
using Random = UnityEngine.Random;

namespace Game.Units
{
    [RequireComponent(typeof(NavMeshAgent))]
    public class UnitMeshAgent : MonoBehaviour, ITargetable
    {
        private float _distanceToGroup;
        
        private NavMeshAgent _navMeshAgent;
        private bool _destinationRandomized = false;
        
        [Inject]
        public void Construct(float distanceToGroup) => _distanceToGroup = distanceToGroup;

        private void Awake() => _navMeshAgent = GetComponent<NavMeshAgent>();

        private void Update() => RandomizeAgentDestinations();

        public bool TryAcceptPoint(GameObject point)
        {
            _destinationRandomized = false;
            return _navMeshAgent.SetDestination(point.transform.position);
        }

        private void RandomizeAgentDestinations()
        {
            if (_navMeshAgent.hasPath && _destinationRandomized == false && _navMeshAgent.remainingDistance < _distanceToGroup)
            {
                _navMeshAgent.SetDestination(transform.position + Random.insideUnitSphere * _distanceToGroup);
                _destinationRandomized = true;
            }
        }
    }
}