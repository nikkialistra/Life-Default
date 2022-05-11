using Animancer;
using Animancer.FSM;
using Sirenix.OdinInspector;
using UnityEngine;

namespace Units.Humans.Animations.States
{
    [EventNames(HitEventName)]
    public class AttackState : HumanState
    {
        [Required]
        [SerializeField] private UnitAttacker _unitAttacker;

        private const string HitEventName = "Hit";
        
        public float Speed
        {
            set => _clip.Speed = value;
        }
        
        public override AnimationType AnimationType => AnimationType.Attack;

        public override bool CanEnterState
        {
            get
            {
                return StateChange<HumanState>.PreviousState.AnimationType switch
                {
                    AnimationType.Die => false,
                    _ => true
                };
            }
        }

        public override void OnEnterState()
        {
            base.OnEnterState();
            
            _clip.Events.SetCallback(HitEventName, Hit);
        }
        
        private void Hit()
        {
            _unitAttacker.Hit(GetHitTime());
        }
        
        private float GetHitTime()
        {
            return _clip.Events[HitEventName].normalizedTime * _clip.Clip.length;
        }
    }
}
