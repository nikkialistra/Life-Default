using System;
using Animancer;
using Sirenix.OdinInspector;
using UnityEngine;

namespace Humans.Animations.States
{
    public class DieState : HumanState
    {
        public Action EndAction { get; set; }

        public override AnimationType AnimationType => AnimationType.Die;

        public override bool CanEnterState => true;

        [Required]
        [SerializeField] private ClipTransition _clip;

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
