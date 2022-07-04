using System.Collections;
using Animancer;
using Infrastructure.Settings;
using Sirenix.OdinInspector;
using UnityEngine;
using Zenject;

namespace Units.Humans.Animations.States
{
    [EventNames(HitEvent, HitEndEvent)]
    [RequireComponent(typeof(UnitEquipment))]
    public class AttackState : HumanState
    {
        private const string HitEvent = "Hit";
        private const string HitEndEvent = "Hit End";

        private const int AttackIdle = 0;
        private const int Attack = 1;

        private const int AttackDownward = 0;
        private const int AttackHorizontal = 1;
        private const int AttackSlash = 2;

        [Title("Melee")]
        [Required]
        [SerializeField] private ClipTransition _meleeIdleClip;
        [Required]
        [SerializeField] private LinearMixerTransition _meleeClips;
        [Space]
        [SerializeField] private float _meleeTimeBetweenStrikesPercent = 0.16f;

        [Title("Ranged")]
        [Required]
        [SerializeField] private ClipTransition _rangedIdleClip;
        [Required]
        [SerializeField] private LinearMixerTransition _rangedClips;
        [Space]
        [SerializeField] private float _rangedTimeBetweenStrikesPercent = 0.2f;

        private float _meleeTimeBetweenStrikes;
        private float _rangedTimeBetweenStrikes;

        private LowerBodyMoving _lowerBodyMoving;

        private LinearMixerState _attackCycleState;
        private MixerParameterTweenFloat _attackCycleTween;

        private UnitMeshAgent _unitMeshAgent;
        private UnitAttacker _unitAttacker;

        private AnimationSettings _animationSettings;

        [Inject]
        public void Construct(UnitMeshAgent unitMeshAgent, UnitAttacker unitAttacker,
            AnimationSettings animationSettings)
        {
            _unitMeshAgent = unitMeshAgent;
            _unitAttacker = unitAttacker;

            _animationSettings = animationSettings;
        }

        protected override void OnAwake()
        {
            _lowerBodyMoving = new LowerBodyMoving(this, _unitMeshAgent, _humanAnimations, _animationSettings);
        }

        private void Start()
        {
            _attackCycleState = new LinearMixerState();
            _attackCycleTween = new MixerParameterTweenFloat(_attackCycleState);

            _attackCycleState.Initialize(2);
            _attackCycleState.DontSynchronizeChildren();

            _attackCycleState.CreateChild(0, _meleeIdleClip, 0);
            _attackCycleState.CreateChild(1, _meleeClips, 1);
        }

        private void OnEnable()
        {
            _meleeClips.Events.SetCallback(HitEvent, Hit);
            _meleeClips.Events.SetCallback(HitEndEvent, OnMeleeHitEnd);
        }

        private void OnDisable()
        {
            _meleeClips.Events.RemoveCallback(HitEvent, Hit);
            _meleeClips.Events.RemoveCallback(HitEndEvent, OnMeleeHitEnd);
        }

        public override void OnEnterState()
        {
            _lowerBodyMoving.Start();

            _attackCycleState.Parameter = Attack;
            _animancer.Layers[AnimationLayers.Main].Play(_attackCycleState);

            SetMeleeRandomAnimation();
        }

        public override void OnExitState()
        {
            _lowerBodyMoving.Stop();
        }

        public override AnimationType AnimationType => AnimationType.Attack;

        public void SetMeleeAttackSpeed(float value)
        {
            _meleeTimeBetweenStrikes = value * _meleeTimeBetweenStrikesPercent;

            _meleeClips.Speed = _meleeClips.MaximumDuration / TimeForMeleeHit(value);
        }

        public void SetRangedAttackSpeed(float value)
        {
            _rangedTimeBetweenStrikes = value * _rangedTimeBetweenStrikesPercent;

            _rangedClips.Speed = _rangedClips.MaximumDuration / TimeForRangedHit(value);
        }

        private float TimeForMeleeHit(float value)
        {
            return value * (1 - _meleeTimeBetweenStrikesPercent);
        }

        private float TimeForRangedHit(float value)
        {
            return value * (1 - _rangedTimeBetweenStrikesPercent);
        }

        private void OnMeleeHitEnd()
        {
            if (_humanAnimations.TryStopIfNotAttacking()) return;

            StartCoroutine(CSetMeleeRandomAnimationAfter());
        }

        private IEnumerator CSetMeleeRandomAnimationAfter()
        {
            _attackCycleTween.Start(AttackIdle, _meleeIdleClip.FadeDuration);

            yield return new WaitForSeconds(_meleeTimeBetweenStrikes);

            SetMeleeRandomAnimation();
            _attackCycleTween.Start(Attack, _meleeIdleClip.FadeDuration);
        }

        private void SetMeleeRandomAnimation()
        {
            _meleeClips.State.Parameter = Random.Range(AttackDownward, AttackSlash + 1);
            _meleeClips.State.NormalizedTime = 0;
        }

        private void Hit()
        {
            _unitAttacker.Hit();
        }
    }
}
