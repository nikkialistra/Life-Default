using System;
using Pathfinding;
using UnityEngine;
using Random = UnityEngine.Random;

namespace Enemies.Enemy
{
    [RequireComponent(typeof(EnemyFacade))]
    [RequireComponent(typeof(AIPath))]
    public class EnemyMeshAgent : MonoBehaviour
    {
        private EnemyFacade _enemyFacade;
        private AIPath _aiPath;

        private void Awake()
        {
            _enemyFacade = GetComponent<EnemyFacade>();
            _aiPath = GetComponent<AIPath>();
        }

        public event Action DestinationReach;

        public float Velocity => _aiPath.velocity.magnitude;

        private void OnEnable()
        {
            _enemyFacade.Spawn += Activate;
            _enemyFacade.Die += Deactivate;
        }

        private void OnDisable()
        {
            _enemyFacade.Spawn -= Activate;
            _enemyFacade.Die -= Deactivate;
        }

        public void Activate()
        {
            _aiPath.isStopped = false;
        }

        public void Deactivate()
        {
            _aiPath.isStopped = true;
        }

        public void GoInRadius(float radius)
        {
            var randomPosition = transform.position + RandomPointOnCircle(radius);

            _aiPath.destination = randomPosition;
        }

        public void StopMoving()
        {
            _aiPath.isStopped = true;
            DestinationReach?.Invoke();
        }

        private Vector3 RandomPointOnCircle(float radius)
        {
            var point = Random.insideUnitCircle * radius;
            return new Vector3(point.x, 0, point.y);
        }
    }
}
