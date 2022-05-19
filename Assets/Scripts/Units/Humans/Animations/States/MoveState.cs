using System.Collections;
using Animancer;
using Animancer.FSM;
using Sirenix.OdinInspector;
using UnityEngine;

namespace Units.Humans.Animations.States
{
    public class MoveState : HumanState
    {
        [Required]
        [SerializeField] private ManualMixerTransition _clips;

        [Space]
        [Required]
        [SerializeField] private UnitAttacker _unitAttacker;

        private const int Move = 0;
        private const int MoveWithWeapon = 1;

        private Coroutine _choosingClipCoroutine;

        public override AnimationType AnimationType => AnimationType.Move;

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
                if (_unitAttacker.HoldingWeapon)
                {
                    SetMoveAnimation();
                }
                else
                {
                    SetMoveWithWeaponAnimation();
                }
                
                yield return null;
            }
        }

        private void SetMoveAnimation()
        {
            _clips.State.ChildStates[Move].Weight = 0;
            _clips.State.ChildStates[MoveWithWeapon].Weight = 1;
        }

        private void SetMoveWithWeaponAnimation()
        {
            _clips.State.ChildStates[Move].Weight = 1;
            _clips.State.ChildStates[MoveWithWeapon].Weight = 0;
        }
    }
}
