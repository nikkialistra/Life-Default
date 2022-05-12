using System;
using Animancer;
using Animancer.FSM;
using Sirenix.OdinInspector;
using Units.Humans.Animations;
using Units.Humans.Animations.States;
using UnityEngine;

namespace Units.Humans
{
    [RequireComponent(typeof(AnimancerComponent))]
    [RequireComponent(typeof(IdleState))]
    [RequireComponent(typeof(MoveState))]
    [RequireComponent(typeof(AttackState))]
    [RequireComponent(typeof(DieState))]
    public class HumanAnimations : MonoBehaviour
    {
        [Title("Masks")]
        [Required]
        [SerializeField] private AvatarMask _fullMask;
        [Required]
        [SerializeField] private AvatarMask _upperBodyMask;

        private IdleState _idleState;
        private MoveState _moveState;
        private AttackState _attackState;

        private DieState _dieState;

        private AnimancerComponent _animancer;

        private void Awake()
        {
            _idleState = GetComponent<IdleState>();
            _moveState = GetComponent<MoveState>();
            _attackState = GetComponent<AttackState>();

            _dieState = GetComponent<DieState>();
            
            _animancer = GetComponent<AnimancerComponent>();
        }

        public StateMachine<HumanState> StateMachine { get; } = new();

        public void Move()
        {
            StateMachine.TrySetState(_moveState);
        }
        
        public void Idle()
        {
            StateMachine.TrySetState(_idleState);
        }

        public void ForceIdle()
        {
            StateMachine.ForceSetState(_idleState);
        }

        public void Attack()
        {
            StateMachine.TrySetState(_attackState);
        }

        public void StopAttack()
        {
            StateMachine.ForceSetState(_idleState);
        }

        public void SetAttackSpeed(float value)
        {
            _attackState.Speed = value;
        }

        public void Die(Action died)
        {
            _dieState.EndAction = died;
            StateMachine.TrySetState(_dieState);
        }

        public void SetFullMaskForActions()
        {
            _animancer.Layers[AnimationLayers.Actions].SetMask(_fullMask);
        }

        public void SetUpperBodyMaskForActions()
        {
            _animancer.Layers[AnimationLayers.Actions].SetMask(_upperBodyMask);
        }
    }
}
