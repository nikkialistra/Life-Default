﻿using Animancer;
using Animancer.FSM;
using Colonists;
using Sirenix.OdinInspector;
using UnityEngine;

namespace Units.Humans.Animations.States
{
    [EventNames(HitEvent)]
    public class GatherResourceState : HumanState
    {
        [Required]
        [SerializeField] private ColonistGatherer _colonistGatherer;
        [Required]
        [SerializeField] private UnitEquipment _unitEquipment;

        private const string HitEvent = "Hit";

        public override AnimationType AnimationType => AnimationType.GatherResource;
        
        public override bool CanEnterState
        {
            get
            {
                return StateChange<HumanState>.PreviousState.AnimationType switch
                {
                    AnimationType.Die => false,
                    _ => true
                };
            }
        }

        public override void OnEnterState()
        {
            base.OnEnterState();
            
            _clip.Events.SetCallback(HitEvent, Hit);
        }

        public override void OnExitState()
        {
            base.OnExitState();
            
            _unitEquipment.UnequipInstrument();
        }

        private void Hit()
        {
            _colonistGatherer.Hit(GetHitTime());
        }

        private float GetHitTime()
        {
            return _clip.Events[HitEvent].normalizedTime * _clip.Clip.length;
        }
    }
}
