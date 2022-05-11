using System;
using Animancer;
using Animancer.FSM;
using UnityEngine;

namespace Units.Humans.Animations
{
    [RequireComponent(typeof(AnimancerComponent))]
    public abstract class HumanState : MonoBehaviour, IState
    {
        [SerializeField] protected ClipTransition _clip;
        [SerializeField] private AnimationLayer _layer = AnimationLayer.Main;

        private AnimancerComponent _animancerComponent;

        private void Awake()
        {
            _animancerComponent = GetComponent<AnimancerComponent>();
        }

        private enum AnimationLayer
        {
            Main,
            Action
        }

        public virtual AnimationType AnimationType =>
            throw new InvalidOperationException("Cannot get animation type of base human state");

        public virtual bool CanEnterState => true;

        public virtual bool CanExitState => true;

        public virtual void OnEnterState()
        {
            if (_layer == AnimationLayer.Main)
            {
                _animancerComponent.Play(_clip);
            }
            else
            {
                _animancerComponent.Layers[AnimationLayers.Actions].Play(_clip);
            }
        }

        public virtual void OnExitState()
        {
            
        }
    }
}
