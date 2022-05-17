using System;
using Animancer;
using Animancer.FSM;
using UnityEngine;

namespace Units.Humans.Animations.Actions.States
{
    [RequireComponent(typeof(AnimancerComponent))]
    public abstract class ActionsHumanState : MonoBehaviour, IState
    {
        [SerializeField] protected ClipTransition _clip;

        protected AnimancerComponent _animancer;

        private void Awake()
        {
            _animancer = GetComponent<AnimancerComponent>();
        }

        public virtual ActionsAnimationType ActionsAnimationType =>
            throw new InvalidOperationException("Cannot get actions animation type of base human state");

        public virtual bool CanEnterState => true;

        public virtual bool CanExitState => true;

        public virtual void OnEnterState()
        {
            _animancer.Layers[AnimationLayers.Actions].Weight = 0;
            _animancer.Layers[AnimationLayers.Actions].Play(_clip);
            _animancer.Layers[AnimationLayers.Actions].StartFade(1f, _clip.FadeDuration);
        }

        public virtual void OnExitState()
        {

        }
    }
}
