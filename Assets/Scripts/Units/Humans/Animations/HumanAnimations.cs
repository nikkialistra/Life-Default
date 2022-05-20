﻿using System;
using Animancer;
using Animancer.FSM;
using Sirenix.OdinInspector;
using Units.Humans.Animations.States;
using UnityEngine;

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
        [SerializeField] private UnitMeshAgent _unitMeshAgent;
        [Required]
        [SerializeField] private ClipTransition _lowerBodyOverwriteMoveClip;
        [Space]
        [Required]
        [SerializeField] private AvatarMask _lowerBodyMask;
        
        private AnimancerComponent _animancer;

        private IdleState _idleState;
        private MoveState _moveState;
        
        private AttackState _attackState;

        private DieState _dieState;

        private bool _attacking;

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

        public void SetAttackSpeed(float value)
        {
            _attackState.Speed = value;
        }

        public void Die(Action died)
        {
            _dieState.EndAction = died;
            TrySetState(_dieState);
        }

        public void ActivateLowerBodyOverwrite()
        {
            _animancer.Layers[AnimationLayers.LowerBodyOverwrite].Play(_lowerBodyOverwriteMoveClip);
            _animancer.Layers[AnimationLayers.LowerBodyOverwrite].Weight = 0;
        }
        
        public void DeactivateLowerBodyOverwrite()
        {
            _animancer.Layers[AnimationLayers.LowerBodyOverwrite].Stop();
        }

        public void LowerBodyOverwriteToMove()
        {
            _animancer.Layers[AnimationLayers.LowerBodyOverwrite].Play(_lowerBodyOverwriteMoveClip);
            _animancer.Layers[AnimationLayers.LowerBodyOverwrite].StartFade(1f, _lowerBodyOverwriteMoveClip.FadeDuration);
        }
        
        public void LowerBodyOverwriteToIdle()
        {
            _animancer.Layers[AnimationLayers.LowerBodyOverwrite].Play(_lowerBodyOverwriteMoveClip);
            _animancer.Layers[AnimationLayers.LowerBodyOverwrite].StartFade(0f, _lowerBodyOverwriteMoveClip.FadeDuration);
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
