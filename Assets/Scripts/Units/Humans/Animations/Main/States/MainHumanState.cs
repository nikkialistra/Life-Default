using System;
using Animancer;
using Animancer.FSM;
using UnityEngine;

namespace Units.Humans.Animations.Main.States
{
    [RequireComponent(typeof(AnimancerComponent))]
    public abstract class MainHumanState : MonoBehaviour, IState
    {
        [SerializeField] protected ClipTransition _clip;

        private AnimancerComponent _animancer;

        private void Awake()
        {
            _animancer = GetComponent<AnimancerComponent>();
        }

        public virtual MainAnimationType MainAnimationType =>
            throw new InvalidOperationException("Cannot get main animation type of base human state");

        public virtual bool CanEnterState => true;

        public virtual bool CanExitState => true;

        public virtual void OnEnterState()
        {
            _animancer.Layers[AnimationLayers.Main].Play(_clip);
        }

        public virtual void OnExitState()
        {

        }
    }
}
