using System;
using Sirenix.OdinInspector;
using Units;
using UnityEngine;

namespace Enemies
{
    [RequireComponent(typeof(EntityAnimator))]
    public class EnemyAnimator : MonoBehaviour
    {
        [Required]
        [SerializeField] private EnemyMeshAgent _enemyMeshAgent;

        private EntityAnimator _entityAnimator;

        private void Awake()
        {
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
