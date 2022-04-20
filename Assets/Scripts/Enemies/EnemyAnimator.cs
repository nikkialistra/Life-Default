using System;
using Sirenix.OdinInspector;
using Units;
using UnityEngine;

namespace Enemies
{
    [RequireComponent(typeof(UnitAnimator))]
    public class EnemyAnimator : MonoBehaviour
    {
        [Required]
        [SerializeField] private EnemyMeshAgent _enemyMeshAgent;

        private UnitAnimator _unitAnimator;

        private void Awake()
        {
            _unitAnimator = GetComponent<UnitAnimator>();
        }

        private void Update()
        {
            _unitAnimator.Move(_enemyMeshAgent.IsMoving);
        }

        public void Die(Action died)
        {
            _unitAnimator.Die(died);
        }
    }
}
