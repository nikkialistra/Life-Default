using System;
using Animancer;
using Animancer.FSM;
using UnityEngine;

namespace Units.Humans.Animations
{
    [RequireComponent(typeof(AnimancerComponent))]
    public class HumanState : MonoBehaviour, IState
    {
        [SerializeField] private ClipTransition _clip;

        private AnimancerComponent _animancerComponent;

        private void Awake()
        {
            _animancerComponent = GetComponent<AnimancerComponent>();
        }

        public virtual AnimationType AnimationType =>
            throw new InvalidOperationException("Cannot get animation type of base human state");

        public virtual bool CanEnterState => true;
        public virtual bool CanExitState => true;

        public virtual void OnEnterState()
        {
            _animancerComponent.Play(_clip);
        }

        public virtual void OnExitState()
        {
            
        }
    }
}
