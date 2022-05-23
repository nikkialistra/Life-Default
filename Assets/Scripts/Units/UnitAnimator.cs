using System;
using Sirenix.OdinInspector;
using Units.Humans.Animations;
using UnityEngine;

namespace Units
{
    public class UnitAnimator : MonoBehaviour
    {
        [Required]
        [SerializeField] private HumanAnimations _humanAnimations;

        public void Idle()
        {
            _humanAnimations.Idle();
        }

        public void Move()
        {
            _humanAnimations.Move();
        }

        public void Attack()
        {
            _humanAnimations.Attack();
        }

        public void FinishAttack()
        {
            _humanAnimations.StopAttackOnAnimationEnd();
        }

        public void StopAttack()
        {
            _humanAnimations.StopActions();
        }

        public void SetMeleeAttackSpeed(float value)
        {
            _humanAnimations.SetMeleeAttackSpeed(value);
        }

        public void SetRangedAttackSpeed(float value)
        {
            _humanAnimations.SetRangedAttackSpeed(value);
        }

        public void Die(Action died)
        {
            _humanAnimations.Die(died);
        }
    }
}
