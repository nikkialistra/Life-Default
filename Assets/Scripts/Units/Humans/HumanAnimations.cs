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
        [SerializeField] private GatherResourceState _cutTreesState;
        [SerializeField] private GatherResourceState _mineRocksState;
        
        [Title("Masks")]
        [Required]
        [SerializeField] private AvatarMask _fullMask;
        [Required]
        [SerializeField] private AvatarMask _upperBodyMask;

        private IdleState _idleState;
        private MoveState _moveState;
        private AttackState _attackState;

        private DieState _dieState;

        private readonly StateMachine<HumanState> _stateMachine = new();
        
        private AnimancerComponent _animancer;

        private void Awake()
        {
            _idleState = GetComponent<IdleState>();
            _moveState = GetComponent<MoveState>();
            _attackState = GetComponent<AttackState>();

            _dieState = GetComponent<DieState>();
            
            _animancer = GetComponent<AnimancerComponent>();
        }

        private void OnEnable()
        {
            _idleState.Enter += SetFullMaskForActions;
            _moveState.Enter += SetUpperBodyMaskForActions;
        }

        private void OnDisable()
        {
            _idleState.Enter -= SetFullMaskForActions;
            _moveState.Enter -= SetUpperBodyMaskForActions;
        }

        private void Start()
        {
            _stateMachine.ForceSetState(_idleState);
            _idleState.enabled = true;
        }

        public void Move()
        {
            _stateMachine.TrySetState(_moveState);
        }
        
        public void Idle()
        {
            _stateMachine.TrySetState(_idleState);
        }

        public void Attack()
        {
            _stateMachine.TrySetState(_attackState);
        }

        public void StopAttack()
        {
            _stateMachine.ForceSetState(_idleState);
        }

        public void SetAttackSpeed(float value)
        {
            _attackState.Speed = value;
        }

        public void MineRocks()
        {
            _stateMachine.TrySetState(_mineRocksState);
        }

        public void CutTrees()
        {
            _stateMachine.TrySetState(_cutTreesState);
        }

        public void StopGathering()
        {
            _stateMachine.ForceSetState(_idleState);
        }

        public void Die(Action died)
        {
            _dieState.EndAction = died;
            _stateMachine.TrySetState(_dieState);
        }
        
        private void SetFullMaskForActions()
        {
            _animancer.Layers[AnimationLayers.Actions].SetMask(_fullMask);
        }

        private void SetUpperBodyMaskForActions()
        {
            _animancer.Layers[AnimationLayers.Actions].SetMask(_upperBodyMask);
        }
    }
}
