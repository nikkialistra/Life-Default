using Animancer;
using Animancer.FSM;
using Colonists;
using Sirenix.OdinInspector;
using Units.Humans.Animations.Main;
using Units.Humans.Animations.Main.States;
using UnityEngine;

namespace Units.Humans.Animations.Actions.States
{
    [EventNames(HitEvent)]
    public class GatherResourceState : ActionsHumanState
    {
        [Required]
        [SerializeField] private ColonistGatherer _colonistGatherer;
        [Required]
        [SerializeField] private UnitEquipment _unitEquipment;

        private const string HitEvent = "Hit";

        public override ActionsAnimationType ActionsAnimationType => ActionsAnimationType.GatherResource;

        private void OnEnable()
        {
            _clip.Events.SetCallback(HitEvent, Hit);
        }

        private void OnDisable()
        {
            _clip.Events.RemoveCallback(HitEvent, Hit);
        }

        public override bool CanEnterState
        {
            get
            {
                return StateChange<MainHumanState>.PreviousState.MainAnimationType switch
                {
                    MainAnimationType.Die => false,
                    _ => true
                };
            }
        }

        public override void OnExitState()
        {
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
