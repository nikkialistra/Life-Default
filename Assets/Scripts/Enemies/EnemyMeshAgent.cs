using Units;
using UnityEngine;

namespace Enemies
{
    [RequireComponent(typeof(UnitMeshAgent))]
    public class EnemyMeshAgent : MonoBehaviour
    {
        private UnitMeshAgent _unitMeshAgent;

        private void Awake()
        {
            _unitMeshAgent = GetComponent<UnitMeshAgent>();
        }

        public bool IsMoving => _unitMeshAgent.IsMoving;
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
        }

        public void StopMoving()
        {
            _unitMeshAgent.StopMoving();
        }

        private void OnDestinationReach()
        {
            Idle = true;
        }
    }
}
