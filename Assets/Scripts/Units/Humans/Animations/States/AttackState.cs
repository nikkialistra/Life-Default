using Animancer;
using Sirenix.OdinInspector;
using UnityEngine;

namespace Units.Humans.Animations.States
{
    [EventNames(HitEvent, HitEndEvent)]
    [RequireComponent(typeof(UnitEquipment))]
    public class AttackState : HumanState
    {
        [Required]
        [SerializeField] private ClipTransition _clip;
        
        [Space]
        [Required]
        [SerializeField] private UnitMeshAgent _unitMeshAgent;
        [Required]
        [SerializeField] private UnitAttacker _unitAttacker;

        private const string HitEvent = "Hit";
        private const string HitEndEvent = "Hit End";

        private LowerBodyMoving _lowerBodyMoving;

        protected override void OnAwake()
        {
            _lowerBodyMoving = new LowerBodyMoving(this, _unitMeshAgent, _humanAnimations);
        }

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
        
        public override void OnEnterState()
        {
            _lowerBodyMoving.Start();
            
            _animancer.Layers[AnimationLayers.Main].Play(_clip);
        }

        public override void OnExitState()
        {
            _lowerBodyMoving.Stop();
        }

        public override AnimationType AnimationType => AnimationType.Attack;

        public float Speed
        {
            set => _clip.Speed = value;
        }

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
