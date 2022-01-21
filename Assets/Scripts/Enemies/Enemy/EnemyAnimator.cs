using System;
using System.Collections;
using Sirenix.OdinInspector;
using UnityEngine;

namespace Enemies.Enemy
{
    public class EnemyAnimator : MonoBehaviour
    {
        [Required]
        [SerializeField] private Animator _animator;

        private readonly int _velocity = Animator.StringToHash("velocity");
        private readonly int _death = Animator.StringToHash("death");

        private EnemyMeshAgent _enemyMeshAgent;

        private void Awake()
        {
            _enemyMeshAgent = GetComponent<EnemyMeshAgent>();
        }

        private void Update()
        {
            SetAnimatorVelocity();
        }

        public void Die(Action died)
        {
            _animator.SetTrigger(_death);
            StartCoroutine(WaitDeathFinish(died));
        }

        private IEnumerator WaitDeathFinish(Action died)
        {
            if (!_animator.GetCurrentAnimatorStateInfo(0).IsName("Death"))
            {
                yield return null;
            }

            while (_animator.GetCurrentAnimatorStateInfo(0).normalizedTime < 1.5f)
            {
                yield return null;
            }

            died();
        }

        private void SetAnimatorVelocity()
        {
            _animator.SetFloat(_velocity, _enemyMeshAgent.Velocity);
        }
    }
}
