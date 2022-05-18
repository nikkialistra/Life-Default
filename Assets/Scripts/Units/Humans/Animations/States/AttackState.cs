using Animancer;
using Sirenix.OdinInspector;
using UnityEngine;

namespace Units.Humans.Animations.States
{
    [EventNames(HitEvent, HitEndEvent)]
    public class AttackState : HumanState
    {
        [Title("Components")]
        [Required]
        [SerializeField] private UnitAttacker _unitAttacker;
        [Required]
        [SerializeField] private HumanAnimations _humanAnimations;

        private const string HitEvent = "Hit";
        private const string HitEndEvent = "Hit End";

        private Coroutine _updatingMovingCoroutine;

        private Coroutine _idleCoroutine;

        private void OnEnable()
        {
            _clip.Events.SetCallback(HitEvent, Hit);
            _clip.Events.SetCallback(HitEndEvent, _humanAnimations.StopIfNotAttacking);
        }

        private void OnDisable()
        {
            _clip.Events.RemoveCallback(HitEvent, Hit);
            _clip.Events.RemoveCallback(HitEndEvent, _humanAnimations.StopIfNotAttacking);
        }

        public float Speed
        {
            set => _clip.Speed = value;
        }
        
        public override AnimationType AnimationType => AnimationType.Attack;

        private void Hit()
        {
            _unitAttacker.Hit(GetHitTime());
        }

        private float GetHitTime()
        {
            return _clip.Events[HitEvent].normalizedTime * _clip.Clip.length;
        }
    }
}
