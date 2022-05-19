using System;
using Sirenix.OdinInspector;
using Units.Humans;
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

        public void SetAttackSpeed(float value)
        {
            _humanAnimations.SetAttackSpeed(value);
        }

        public void Die(Action died)
        {
            _humanAnimations.Die(died);
        }
    }
}
