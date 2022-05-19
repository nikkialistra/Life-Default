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
                yield return null;
                
                if (_unitAttacker.HoldingWeapon)
                {
                    _clips.State.ChildStates[0].Weight = 0;
                    _clips.State.ChildStates[1].Weight = 1;
                }
                else
                {
                    _clips.State.ChildStates[0].Weight = 1;
                    _clips.State.ChildStates[1].Weight = 0;
                }
                
                yield return null;
            }
        }
    }
}
