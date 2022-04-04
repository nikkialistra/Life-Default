using System;
using Entities.Creature;
using UnityEngine;

namespace Colonists.Colonist
{
    [RequireComponent(typeof(EntityAnimator))]
    [RequireComponent(typeof(Animator))]
    public class ColonistAnimator : MonoBehaviour
    {
        private EntityAnimator _entityAnimator;
        private Animator _animator;

        private readonly int _interacting = Animator.StringToHash("interacting");
        private readonly int _attacking = Animator.StringToHash("attacking");

        private void Awake()
        {
            _entityAnimator = GetComponent<EntityAnimator>();
            _animator = GetComponent<Animator>();
        }

        public void Move(bool value)
        {
            _entityAnimator.Move(value);
        }

        public void Interact(bool value)
        {
            _animator.SetBool(_interacting, value);
        }

        public void Attack(bool value)
        {
            _animator.SetBool(_attacking, value);
        }

        public void Die(Action died)
        {
            _entityAnimator.Die(died);
        }
    }
}
