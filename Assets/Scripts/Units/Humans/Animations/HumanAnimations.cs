using System;
using System.Collections.Generic;
using Animancer;
using Animancer.FSM;
using Sirenix.OdinInspector;
using Units.Humans.Animations.States;
using UnityEngine;
using Zenject;
using Random = UnityEngine.Random;

namespace Units.Humans.Animations
{
    [RequireComponent(typeof(AnimancerComponent))]
    [RequireComponent(typeof(IdleState))]
    [RequireComponent(typeof(MoveState))]
    [RequireComponent(typeof(AttackState))]
    [RequireComponent(typeof(DieState))]
    public class HumanAnimations : MonoBehaviour
    {
        [Required]
        [SerializeField] private ClipTransition _lowerBodyOverwriteMoveClip;
        [Space]
        [Required]
        [SerializeField] private AvatarMask _lowerBodyMask;
        [Space]
        [SerializeField] private float _effectsWeight = 0.3f;
        [Required]
        [SerializeField] private List<ClipTransition> _hitClips;

        private AnimancerComponent _animancer;

        private IdleState _idleState;
        private MoveState _moveState;

        private AttackState _attackState;

        private DieState _dieState;

        private bool _attacking;

        private UnitMeshAgent _unitMeshAgent;

        [Inject]
        public void Construct(UnitMeshAgent unitMeshAgent)
        {
            _unitMeshAgent = unitMeshAgent;
        }

        private void Awake()
        {
            _animancer = GetComponent<AnimancerComponent>();

            _idleState = GetComponent<IdleState>();
            _moveState = GetComponent<MoveState>();
            _attackState = GetComponent<AttackState>();
            _dieState = GetComponent<DieState>();
        }

        private void Start()
        {
            _animancer.Layers[AnimationLayers.LowerBodyOverwrite].SetMask(_lowerBodyMask);
            _animancer.Layers[AnimationLayers.LowerBodyOverwrite].Weight = 0;

            _animancer.Layers[AnimationLayers.Effects].IsAdditive = true;
        }

        private StateMachine<HumanState> StateMachine { get; } = new();

        public void Move()
        {
            TrySetState(_moveState);
        }

        public void Idle()
        {
            TrySetState(_idleState);
        }

        public void Attack()
        {
            _attacking = true;
            TrySetState(_attackState);
        }

        public void StopAttackOnAnimationEnd()
        {
            _attacking = false;
        }

        public void StopActions()
        {
            if (_unitMeshAgent.IsMoving)
            {
                ForceSetState(_moveState);
            }
            else
            {
                ForceSetState(_idleState);
            }
        }

        public bool TryStopIfNotAttacking()
        {
            if (!_attacking)
            {
                StopActions();
                return true;
            }

            return false;
        }

        public void SetMeleeAttackSpeed(float value)
        {
            _attackState.SetMeleeAttackSpeed(value);
        }
        
        public void SetRangedAttackSpeed(float value)
        {
            _attackState.SetRangedAttackSpeed(value);
        }

        public void Die(Action died)
        {
            _dieState.EndAction = died;
            TrySetState(_dieState);
        }

        public void LowerBodyOverwriteToMove()
        {
            _animancer.Layers[AnimationLayers.LowerBodyOverwrite].Play(_lowerBodyOverwriteMoveClip);
            _animancer.Layers[AnimationLayers.LowerBodyOverwrite]
                .StartFade(1f, _lowerBodyOverwriteMoveClip.FadeDuration);
        }

        public void LowerBodyOverwriteToIdle()
        {
            if (_animancer.Layers[AnimationLayers.LowerBodyOverwrite].IsPlayingClip(_lowerBodyOverwriteMoveClip.Clip))
            {
                _animancer.Layers[AnimationLayers.LowerBodyOverwrite]
                    .StartFade(0f, _lowerBodyOverwriteMoveClip.FadeDuration);
            }
        }

        public void Hit()
        {
            if (_animancer.Layers[AnimationLayers.Effects].IsAnyStatePlaying())
            {
                return;
            }

            _animancer.Layers[AnimationLayers.Effects].Play(_hitClips[Random.Range(0, _hitClips.Count)]);
            _animancer.Layers[AnimationLayers.Effects].StartFade(_effectsWeight);
        }

        public void TrySetState(HumanState state)
        {
            if (StateMachine.CurrentState != state)
            {
                StateMachine.TrySetState(state);
            }
        }

        private void ForceSetState(HumanState state)
        {
            if (StateMachine.CurrentState != state)
            {
                StateMachine.ForceSetState(state);
            }
        }
    }
}
