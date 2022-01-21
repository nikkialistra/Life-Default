using System;
using Pathfinding;
using UnityEngine;

namespace Enemies.Enemy
{
    [RequireComponent(typeof(EnemyFacade))]
    [RequireComponent(typeof(AIPath))]
    public class EnemyMeshAgent : MonoBehaviour
    {
        [SerializeField] private float _rotationSpeedToEntities;
        [SerializeField] private float _interactionDistance = 3f;

        private bool _activated;

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

        public void StopMoving()
        {
            _aiPath.isStopped = true;
            DestinationReach?.Invoke();
        }

        private void Activate()
        {
            _aiPath.isStopped = false;
            _activated = true;
        }

        private void Deactivate()
        {
            _aiPath.isStopped = true;
            _activated = false;
        }
    }
}
