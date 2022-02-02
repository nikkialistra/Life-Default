using System;
using Entities.Entity;
using Sirenix.OdinInspector;
using UnityEngine;

namespace Units.Unit
{
    [RequireComponent(typeof(UnitMeshAgent))]
    [RequireComponent(typeof(EntityAnimator))]
    public class UnitAnimator : MonoBehaviour
    {
        [Required]
        [SerializeField] private Animator _animator;

        private UnitMeshAgent _unitMeshAgent;
        private EntityAnimator _entityAnimator;

        private readonly int _interacting = Animator.StringToHash("interacting");
        private readonly int _attacking = Animator.StringToHash("attacking");

        private void Awake()
        {
            _unitMeshAgent = GetComponent<UnitMeshAgent>();
            _entityAnimator = GetComponent<EntityAnimator>();
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
