using Animancer;
using Sirenix.OdinInspector;
using UnityEngine;
using Random = UnityEngine.Random;

namespace Units.Humans.Animations.States
{
    [EventNames(HitEvent, HitEndEvent)]
    [RequireComponent(typeof(UnitEquipment))]
    public class AttackState : HumanState
    {
        [Required]
        [SerializeField] private ManualMixerTransition _clips;
        
        [Space]
        [Required]
        [SerializeField] private UnitMeshAgent _unitMeshAgent;
        [Required]
        [SerializeField] private UnitAttacker _unitAttacker;

        private const string HitEvent = "Hit";
        private const string HitEndEvent = "Hit End";

        private const int AttackDownward = 0;
        private const int AttackHorizontal = 1;
        private const int AttackSlash = 2;

        private LowerBodyMoving _lowerBodyMoving;

        protected override void OnAwake()
        {
            _lowerBodyMoving = new LowerBodyMoving(this, _unitMeshAgent, _humanAnimations);
        }

        private void OnEnable()
        {
            _clips.Events.SetCallback(HitEvent, Hit);
            _clips.Events.SetCallback(HitEndEvent, OnHitEnd);
        }

        private void OnDisable()
        {
            _clips.Events.RemoveCallback(HitEvent, Hit);
            _clips.Events.RemoveCallback(HitEndEvent, OnHitEnd);
        }

        public override void OnEnterState()
        {
            _lowerBodyMoving.Start();
            
            _animancer.Layers[AnimationLayers.Main].Play(_clips);
            
            SetRandomAnimation();
        }

        public override void OnExitState()
        {
            _lowerBodyMoving.Stop();
        }

        public override AnimationType AnimationType => AnimationType.Attack;

        public float Speed
        {
            set => _clips.Speed = value;
        }
        
        private void OnHitEnd()
        {
            if (_humanAnimations.TryStopIfNotAttacking())
            {
                return;
            }
            
            SetRandomAnimation();
        }
        
        private void SetRandomAnimation()
        {
            switch (Random.Range(AttackDownward, AttackSlash + 1))
            {
                case AttackDownward:
                    SetAttackDownward();
                    break;
                case AttackHorizontal:
                    SetAttackHorizontal();
                    break;
                case AttackSlash:
                    SetAttackSlash();
                    break;
            }
        }
        
        private void SetAttackDownward()
        {
            _clips.State.ChildStates[AttackDownward].StartFade(1f, _clips.FadeDuration);
            _clips.State.ChildStates[AttackDownward].IsPlaying = true;
            _clips.State.ChildStates[AttackHorizontal].StartFade(0f, _clips.FadeDuration);
            _clips.State.ChildStates[AttackSlash].StartFade(0f, _clips.FadeDuration);
        }

        private void SetAttackHorizontal()
        {
            _clips.State.ChildStates[AttackDownward].StartFade(0f, _clips.FadeDuration);
            _clips.State.ChildStates[AttackHorizontal].StartFade(1f, _clips.FadeDuration);
            _clips.State.ChildStates[AttackHorizontal].IsPlaying = true;
            _clips.State.ChildStates[AttackSlash].StartFade(0f, _clips.FadeDuration);
        }

        private void SetAttackSlash()
        {
            _clips.State.ChildStates[AttackDownward].StartFade(0f, _clips.FadeDuration);
            _clips.State.ChildStates[AttackHorizontal].StartFade(0f, _clips.FadeDuration);
            _clips.State.ChildStates[AttackSlash].StartFade(1f, _clips.FadeDuration);
            _clips.State.ChildStates[AttackSlash].IsPlaying = true;
        }

        private void Hit()
        {
            _unitAttacker.Hit(GetHitTime());
        }

        private float GetHitTime()
        {
            return 1f;
            //return _clip.Events[HitEvent].normalizedTime * _clip.Clip.length;
        }
    }
}
