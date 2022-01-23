using System;
using Entities.Entity;
using UnityEngine;
using Random = UnityEngine.Random;

namespace Enemies.Enemy
{
    [RequireComponent(typeof(EntityMeshAgent))]
    public class EnemyMeshAgent : MonoBehaviour
    {
        private EntityMeshAgent _entityMeshAgent;

        public event Action DestinationReach;

        private void Awake()
        {
            _entityMeshAgent = GetComponent<EntityMeshAgent>();
        }

        public float Velocity => _entityMeshAgent.Velocity;

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

        public void GoInRadius(float radius)
        {
            var randomPosition = transform.position + RandomPointOnCircle(radius);

            _entityMeshAgent.SetDestinationToPosition(randomPosition);
        }

        private Vector3 RandomPointOnCircle(float radius)
        {
            var point = Random.insideUnitCircle * radius;
            return new Vector3(point.x, 0, point.y);
        }

        private void OnDestinationReach()
        {
            DestinationReach?.Invoke();
        }
    }
}
