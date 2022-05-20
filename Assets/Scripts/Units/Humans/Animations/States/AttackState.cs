using System;
using System.Collections;
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
        [SerializeField] private LinearMixerTransition _clips;
        [Required]
        [SerializeField] private ClipTransition _idleClip;

        [Space]
        [Required]
        [SerializeField] private UnitMeshAgent _unitMeshAgent;
        [Required]
        [SerializeField] private UnitAttacker _unitAttacker;
        
        [Space]
        [SerializeField] private float _timeBetweenStrikes = 0.4f;

        private const string HitEvent = "Hit";
        private const string HitEndEvent = "Hit End";

        private const int AttackDownward = 0;
        private const int AttackHorizontal = 1;
        private const int AttackSlash = 2;

        private LowerBodyMoving _lowerBodyMoving;
        
        private LinearMixerState _attackCycleState;
        private MixerParameterTweenFloat _attackCycleTween;

        protected override void OnAwake()
        {
            _lowerBodyMoving = new LowerBodyMoving(this, _unitMeshAgent, _humanAnimations);
        }

        private void Start()
        {
            _attackCycleState = new LinearMixerState();
            _attackCycleTween = new MixerParameterTweenFloat(_attackCycleState);
            
            _attackCycleState.Initialize(2);
            _attackCycleState.DontSynchronizeChildren();

            _attackCycleState.CreateChild(0, _clips, 0);
            _attackCycleState.CreateChild(1, _idleClip, 1);
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

            _attackCycleState.Parameter = 0;
            _animancer.Layers[AnimationLayers.Main].Play(_attackCycleState);
            
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
            
            StartCoroutine(SetRandomAnimationAfter());
        }
        
        private IEnumerator SetRandomAnimationAfter()
        {
            _attackCycleTween.Start(1f, _idleClip.FadeDuration);
            
            yield return new WaitForSeconds(_timeBetweenStrikes);
            
            _attackCycleTween.Start(0f, _idleClip.FadeDuration);
            SetRandomAnimation();
        }

        private void SetRandomAnimation()
        {
            _clips.State.Parameter = Random.Range(AttackDownward, AttackSlash + 1);
            _clips.State.NormalizedTime = 0;
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
