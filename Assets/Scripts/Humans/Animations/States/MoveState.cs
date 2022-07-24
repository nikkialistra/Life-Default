using System.Collections;
using Animancer;
using Animancer.FSM;
using Sirenix.OdinInspector;
using Units;
using UnityEngine;

namespace Humans.Animations.States
{
    [RequireComponent(typeof(UnitEquipment))]
    public class MoveState : HumanState
    {
        public override bool CanEnterState
        {
            get
            {
                return StateChange<HumanState>.PreviousState.AnimationType switch
                {
                    AnimationType.Idle => true,
                    _ => false
                };
            }
        }

        public override AnimationType AnimationType => AnimationType.Move;

        [Required]
        [SerializeField] private LinearMixerTransition _clips;

        private UnitEquipment _unitEquipment;

        private const int Move = 0;
        private const int MoveWithWeapon = 1;

        private Coroutine _choosingClipCoroutine;

        protected override void OnAwake()
        {
            _unitEquipment = GetComponent<UnitEquipment>();
        }

        public override void OnEnterState()
        {
            _animancer.Layers[AnimationLayers.Main].Play(_clips);

            _choosingClipCoroutine = StartCoroutine(CChoosingClip());
        }

        public override void OnExitState()
        {
            if (_choosingClipCoroutine != null)
            {
                StopCoroutine(_choosingClipCoroutine);
                _choosingClipCoroutine = null;
            }
        }

        private IEnumerator CChoosingClip()
        {
            while (true)
            {
                _clips.State.Parameter = _unitEquipment.HoldingSomething ? MoveWithWeapon : Move;

                yield return null;
            }
        }
    }
}
