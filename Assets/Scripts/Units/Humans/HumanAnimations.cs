using System;
using Animancer;
using Animancer.FSM;
using Sirenix.OdinInspector;
using Units.Humans.Animations;
using Units.Humans.Animations.Actions.States;
using Units.Humans.Animations.Main.States;
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
        [Required]
        [SerializeField] private UnitMeshAgent _unitMeshAgent;

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

        private bool _attacking;
        
        private bool _fullMaskSet;

        private Vector2 test;

        private void Awake()
        {
            _idleState = GetComponent<IdleState>();
            _moveState = GetComponent<MoveState>();
            _attackState = GetComponent<AttackState>();

            _dieState = GetComponent<DieState>();
            
            _animancer = GetComponent<AnimancerComponent>();
        }

        private void Start()
        {
            _animancer.Layers[AnimationLayers.Actions].SetMask(_upperBodyMask);
        }

        private StateMachine<MainHumanState> MainStateMachine { get; } = new();
        private StateMachine<ActionsHumanState> ActionsStateMachine { get; } = new();

        public void Move()
        {
            TrySetMainState(_moveState);
        }

        public void Idle()
        {
            TrySetMainState(_idleState);
        }

        public void ForceIdle()
        {
            ForceSetMainState(_idleState);
        }

        public void Attack()
        {
            _attacking = true;
            TrySetActionsState(_attackState);
        }

        public void StopAttackOnAnimationEnd()
        {
            _attacking = false;
        }

        public void StopAttack()
        {
            if (_unitMeshAgent.IsMoving)
            {
                ForceSetMainState(_moveState);
            }
            else
            {
                ForceSetMainState(_idleState);
            }
        }

        public void StopIfNotAttacking()
        {
            if (!_attacking)
            {
                StopAttack();
            }
        }

        public void SetAttackSpeed(float value)
        {
            _attackState.Speed = value;
        }

        public void Die(Action died)
        {
            _dieState.EndAction = died;
            TrySetMainState(_dieState);
        }

        public void SetFullMaskForActions()
        {
            if (_fullMaskSet)
            {
                return;
            }
            
            _animancer.Layers[AnimationLayers.Actions].SetMask(_fullMask);

            _fullMaskSet = true;
        }

        public void SetUpperBodyMaskForActions()
        {
            if (!_fullMaskSet)
            {
                return;
            }
            
            _animancer.Layers[AnimationLayers.Actions].SetMask(_upperBodyMask);

            _fullMaskSet = false;
        }

        public void TrySetActionsState(ActionsHumanState state)
        {
            if (ActionsStateMachine.CurrentState != state)
            {
                //Debug.Log("Try set " + state + "from " + MainStateMachine.CurrentState);
                ActionsStateMachine.TrySetState(state);
            }
        }

        private void TrySetMainState(MainHumanState state)
        {
            if (MainStateMachine.CurrentState != state)
            {
                //Debug.Log("Try set " + state + "from " + MainStateMachine.CurrentState);
                MainStateMachine.TrySetState(state);
            }
        }

        private void ForceSetMainState(MainHumanState state)
        {
            if (MainStateMachine.CurrentState != state)
            {
                //Debug.Log("Force " + state + "from " + MainStateMachine.CurrentState);
                MainStateMachine.ForceSetState(state);
            }
        }
    }
}
