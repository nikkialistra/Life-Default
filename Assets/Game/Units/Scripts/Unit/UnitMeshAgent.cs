using Kernel.Types;
using Sirenix.OdinInspector;
using UnityEngine;
using UnityEngine.AI;

namespace Game.Units.Unit
{
    [RequireComponent(typeof(NavMeshAgent))]
    public class UnitMeshAgent : MonoBehaviour, ITargetable
    {
        [MinValue(0)]
        [SerializeField] private float _distanceToGroup;
        
        private NavMeshAgent _navMeshAgent;
        
        private void Awake()
        {
            _navMeshAgent = GetComponent<NavMeshAgent>();
        }

        public bool TryAcceptPoint(GameObject point)
        {
            var destinationSet = _navMeshAgent.SetDestination(point.transform.position + Random.insideUnitSphere * _distanceToGroup);
            return destinationSet;
        }
    }
}