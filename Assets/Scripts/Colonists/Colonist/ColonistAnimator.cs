using System;
using Entities.Creature;
using Sirenix.OdinInspector;
using UnityEngine;

namespace Colonists.Colonist
{
    [RequireComponent(typeof(EntityAnimator))]
    [RequireComponent(typeof(Animator))]
    public class ColonistAnimator : MonoBehaviour
    {
        [Required]
        [SerializeField] private ColonistMeshAgent _colonistMeshAgent;

        private EntityAnimator _entityAnimator;
        private Animator _animator;

        private readonly int _interacting = Animator.StringToHash("interacting");
        private readonly int _attacking = Animator.StringToHash("attacking");

        private void Awake()
        {
            _entityAnimator = GetComponent<EntityAnimator>();
            _animator = GetComponent<Animator>();
        }

        private void Update()
        {
            _entityAnimator.Move(_colonistMeshAgent.IsMoving);
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
