﻿using Animancer;
using Animancer.FSM;
using Sirenix.OdinInspector;
using UnityEngine;

namespace Humans.Animations.States
{
    public class IdleState : HumanState
    {
        public override bool CanEnterState
        {
            get
            {
                if (StateChange<HumanState>.PreviousState == null)
                    return true;

                return StateChange<HumanState>.PreviousState.AnimationType switch
                {
                    AnimationType.Move => true,
                    _ => false
                };
            }
        }

        public override AnimationType AnimationType => AnimationType.Idle;

        [Required]
        [SerializeField] private ClipTransition _clip;

        public override void OnEnterState()
        {
            _animancer.Layers[AnimationLayers.Main].Play(_clip);
        }
    }
}
