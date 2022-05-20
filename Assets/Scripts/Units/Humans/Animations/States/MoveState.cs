using System.Collections;
using Animancer;
using Animancer.FSM;
using Sirenix.OdinInspector;
using UnityEngine;

namespace Units.Humans.Animations.States
{
    [RequireComponent(typeof(UnitEquipment))]
    public class MoveState : HumanState
    {
        [Required]
        [SerializeField] private LinearMixerTransition _clips;

        private UnitEquipment _unitEquipment;

        private const int Move = 0;
        private const int MoveWithWeapon = 1;

        private Coroutine _choosingClipCoroutine;

        public override AnimationType AnimationType => AnimationType.Move;

        protected override void OnAwake()
        {
            _unitEquipment = GetComponent<UnitEquipment>();
        }

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

        public override void OnEnterState()
        {
            _animancer.Layers[AnimationLayers.Main].Play(_clips);

            _choosingClipCoroutine = StartCoroutine(ChoosingClip());
        }

        public override void OnExitState()
        {
            if (_choosingClipCoroutine != null)
            {
                StopCoroutine(_choosingClipCoroutine);
                _choosingClipCoroutine = null;
            }
        }

        private IEnumerator ChoosingClip()
        {
            while (true)
            {
                if (_unitEquipment.HoldingSomething)
                {
                    _clips.State.Parameter = MoveWithWeapon;
                }
                else
                {
                    _clips.State.Parameter = Move;
                }
                
                yield return null;
            }
        }
    }
}
