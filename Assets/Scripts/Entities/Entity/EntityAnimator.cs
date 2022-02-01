using System;
using System.Collections;
using Sirenix.OdinInspector;
using UnityEngine;

namespace Entities.Entity
{
    public class EntityAnimator : MonoBehaviour
    {
        [Required]
        [SerializeField] private Animator _animator;

        private readonly int _isMoving = Animator.StringToHash("isMoving");
        private readonly int _death = Animator.StringToHash("death");

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

        public void SetMoving(bool value)
        {
            _animator.SetBool(_isMoving, value);
        }
    }
}
