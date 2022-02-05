using System;
using System.Collections;
using UnityEngine;

namespace Entities.Creature
{
    [RequireComponent(typeof(Animator))]
    public class EntityAnimator : MonoBehaviour
    {
        private readonly int _moving = Animator.StringToHash("moving");
        private readonly int _death = Animator.StringToHash("death");

        private Animator _animator;

        private void Awake()
        {
            _animator = GetComponent<Animator>();
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

        public void Move(bool value)
        {
            _animator.SetBool(_moving, value);
        }
    }
}
