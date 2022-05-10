using System;
using System.Collections;
using Animancer;
using Animancer.FSM;
using Units.Humans.Animations;
using Units.Humans.Animations.States;
using UnityEngine;

namespace Units.Humans
{
    [RequireComponent(typeof(AnimancerComponent))]
    [RequireComponent(typeof(IdleState))]
    [RequireComponent(typeof(MoveState))]
    [RequireComponent(typeof(DieState))]
    public class HumanAnimations : MonoBehaviour
    {
        [SerializeField] private AnimationClip _attacking;

        private IdleState _idleState;
        private MoveState _moveState;
        private DieState _dieState;

        private readonly StateMachine<HumanState> _stateMachine = new();

        private void Awake()
        {
            _idleState = GetComponent<IdleState>();
            _moveState = GetComponent<MoveState>();
            _dieState = GetComponent<DieState>();
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
            
        }

        public void StopAttack()
        {
            
        }

        public void Die(Action died)
        {
            //var deathState = _animancer.Play(_death);

            //StartCoroutine(WaitDeathFinish(died, deathState));
        }

        private IEnumerator WaitDeathFinish(Action died, AnimancerState deathState)
        {
            while (deathState.NormalizedTime < 1.5f)
            {
                yield return null;
            }

            died();
        }
    }
}
