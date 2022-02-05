using System;
using Entities.Creature;
using UnityEngine;

namespace Enemies.Enemy
{
    [RequireComponent(typeof(EntityAnimator))]
    public class EnemyAnimator : MonoBehaviour
    {
        private EnemyMeshAgent _enemyMeshAgent;
        private EntityAnimator _entityAnimator;

        private void Awake()
        {
            _enemyMeshAgent = GetComponent<EnemyMeshAgent>();
            _entityAnimator = GetComponent<EntityAnimator>();
        }

        private void Update()
        {
            _entityAnimator.Move(_enemyMeshAgent.IsMoving);
        }

        public void Die(Action died)
        {
            _entityAnimator.Die(died);
        }
    }
}
