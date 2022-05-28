using Animancer;
using Colonists;
using Sirenix.OdinInspector;
using UnityEngine;

namespace Units.Humans.Animations.States
{
    [EventNames(HitEvent)]
    [RequireComponent(typeof(UnitEquipment))]
    public class GatherResourceState : HumanState
    {
        [Required]
        [SerializeField] private ClipTransition _clip;
        
        [Space]
        [Required]
        [SerializeField] private ColonistGatherer _colonistGatherer;
        
        private UnitEquipment _unitEquipment;

        private const string HitEvent = "Hit";
        
        protected override void OnAwake()
        {
            _unitEquipment = GetComponent<UnitEquipment>();
        }

        public override AnimationType AnimationType => AnimationType.GatherResource;

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
            _animancer.Layers[AnimationLayers.Main].Play(_clip);
        }

        public override void OnExitState()
        {
            _unitEquipment.UnequipInstrument();
        }

        private void Hit()
        {
            _colonistGatherer.Hit(_clip.MaximumDuration);
        }
    }
}
