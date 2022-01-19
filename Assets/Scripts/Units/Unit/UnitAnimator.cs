using System;
using System.Collections;
using Sirenix.OdinInspector;
using UnityEngine;

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
        private readonly int _interactingWithResource = Animator.StringToHash("interactingWithResource");

        private void Awake()
        {
            _unitMeshAgent = GetComponent<UnitMeshAgent>();
        }

        private void Update()
        {
            SetAnimatorVelocity();
        }

        public void InteractWithResource()
        {
            _animator.SetBool(_interactingWithResource, true);
        }

        public void StopInteractWithResource()
        {
            _animator.SetBool(_interactingWithResource, false);
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
