using System;
using Animancer;
using Sirenix.OdinInspector;
using UnityEngine;

namespace Units.Humans.Animations.States
{
    public class DieState : HumanState
    {
        [Required]
        [SerializeField] private ClipTransition _clip;
        
        public Action EndAction { get; set; }
        
        public override AnimationType AnimationType => AnimationType.Die;

        public override bool CanEnterState => true;

        public override void OnEnterState()
        {
            _animancer.Layers[AnimationLayers.Main].Play(_clip);
            
            _clip.Events.OnEnd = EndAction;
        }

        public override void OnExitState()
        {
            _clip.Events.OnEnd = null;
        }
    }
}
