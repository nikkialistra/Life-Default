using System;
using Entities.Creature;
using Sirenix.OdinInspector;
using UnityEngine;

namespace Units.Unit
{
    [RequireComponent(typeof(EntityAnimator))]
    [RequireComponent(typeof(Animator))]
    public class UnitAnimator : MonoBehaviour
    {
        [Required]
        [SerializeField] private UnitMeshAgent _unitMeshAgent;

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
            _entityAnimator.Move(_unitMeshAgent.IsMoving);
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
