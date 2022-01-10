using System;
using System.Collections;
using Sirenix.OdinInspector;
using UnityEngine;
using UnityEngine.AI;

namespace Units.Unit
{
    [RequireComponent(typeof(UnitMeshAgent))]
    public class UnitAnimator : MonoBehaviour
    {
        [Required]
        [SerializeField] private Animator _animator;

        private UnitMeshAgent _unitMeshAgent;

        private readonly int _velocity = Animator.StringToHash("velocity");
        private readonly int _death = Animator.StringToHash("death");

        private void Awake()
        {
            _unitMeshAgent = GetComponent<UnitMeshAgent>();
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
            _animator.SetFloat(_velocity, _unitMeshAgent.Velocity);
        }
    }
}