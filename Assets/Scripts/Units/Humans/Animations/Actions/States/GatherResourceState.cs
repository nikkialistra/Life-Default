using Animancer;
using Colonists;
using Sirenix.OdinInspector;
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
        [Required]
        [SerializeField] private HumanAnimations _humanAnimations;

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

        public override void OnEnterState()
        {
            base.OnEnterState();
            
            _humanAnimations.SetFullMaskForActions();
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
