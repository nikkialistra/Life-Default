using System;
using Sirenix.OdinInspector;
using Units;
using UnityEngine;
using UnityEngine.Serialization;

namespace Enemies
{
    [RequireComponent(typeof(UnitAnimator))]
    [RequireComponent(typeof(EnemyMeshAgent))]
    public class EnemyAnimator : MonoBehaviour
    { 
        [Required]
        [SerializeField] private Animator _animator;
        
        private readonly int _attacking = Animator.StringToHash("attacking");
        
        private UnitAnimator _unitAnimator;

        private void Awake()
        {
            _unitAnimator = GetComponent<UnitAnimator>();
        }

        public void Move(bool value)
        {
            _unitAnimator.Move(value);
        }
        
        public void Attack(bool value)
        {
            _animator.SetBool(_attacking, value);
        }

        public void Die(Action died)
        {
            _unitAnimator.Die(died);
        }
    }
}
