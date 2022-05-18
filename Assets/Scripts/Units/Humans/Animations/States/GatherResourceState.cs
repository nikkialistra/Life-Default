using Animancer;
using Colonists;
using Sirenix.OdinInspector;
using UnityEngine;

namespace Units.Humans.Animations.States
{
    [EventNames(HitEvent)]
    public class GatherResourceState : HumanState
    {
        [Space]
        [Required]
        [SerializeField] private ColonistGatherer _colonistGatherer;
        [Required]
        [SerializeField] private UnitEquipment _unitEquipment;

        private const string HitEvent = "Hit";

        public override AnimationType AnimationType => AnimationType.GatherResource;

        private void OnEnable()
        {
            _clip.Events.SetCallback(HitEvent, Hit);
        }

        private void OnDisable()
        {
            _clip.Events.RemoveCallback(HitEvent, Hit);
        }

        public override void OnExitState()
        {
            _unitEquipment.Unequip();
            
            base.OnExitState();
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
