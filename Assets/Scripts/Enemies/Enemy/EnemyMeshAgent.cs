using Entities.Creature;
using UnityEngine;

namespace Enemies.Enemy
{
    [RequireComponent(typeof(EntityMeshAgent))]
    public class EnemyMeshAgent : MonoBehaviour
    {
        private EntityMeshAgent _entityMeshAgent;

        private void Awake()
        {
            _entityMeshAgent = GetComponent<EntityMeshAgent>();
        }

        public bool IsMoving => _entityMeshAgent.IsMoving;
        public bool Idle { get; private set; } = true;

        private void OnEnable()
        {
            _entityMeshAgent.DestinationReach += OnDestinationReach;
        }

        private void OnDisable()
        {
            _entityMeshAgent.DestinationReach -= OnDestinationReach;
        }

        public void Deactivate()
        {
            _entityMeshAgent.StopCurrentCommand();
        }

        public void GoToPosition(Vector3 position)
        {
            Idle = false;
            _entityMeshAgent.SetDestinationToPosition(position);
        }

        public void StopMoving()
        {
            _entityMeshAgent.StopMoving();
        }

        private void OnDestinationReach()
        {
            Idle = true;
        }
    }
}
