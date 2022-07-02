using System;
using Animancer;
using Animancer.FSM;
using UnityEngine;

namespace Units.Humans.Animations.States
{
    [RequireComponent(typeof(HumanAnimations))]
    [RequireComponent(typeof(AnimancerComponent))]
    public abstract class HumanState : MonoBehaviour, IState
    {
        public virtual bool CanEnterState => true;

        public virtual bool CanExitState => true;

        public virtual AnimationType AnimationType =>
            throw new InvalidOperationException("Cannot get main animation type of base human state");

        protected HumanAnimations _humanAnimations;
        protected AnimancerComponent _animancer;

        private void Awake()
        {
            _humanAnimations = GetComponent<HumanAnimations>();
            _animancer = GetComponent<AnimancerComponent>();

            OnAwake();
        }

        protected virtual void OnAwake()
        {

        }

        public virtual void OnEnterState()
        {

        }

        public virtual void OnExitState()
        {

        }
    }
}
