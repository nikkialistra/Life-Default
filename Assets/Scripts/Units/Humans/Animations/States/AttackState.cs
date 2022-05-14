using System.Collections;
using Animancer;
using Animancer.FSM;
using Sirenix.OdinInspector;
using UnityEngine;

namespace Units.Humans.Animations.States
{
    [EventNames(HitEventName)]
    public class AttackState : HumanState
    {
        [Required]
        [SerializeField] private UnitAttacker _unitAttacker;
        [Required]
        [SerializeField] private UnitMeshAgent _unitMeshAgent;
        [Required]
        [SerializeField] private HumanAnimations _humanAnimations;
        
        [Space]
        [Required]
        [SerializeField] private ClipTransition _moveClip;
        [Required]
        [SerializeField] private ClipTransition _idleClip;

        private Coroutine _updatingMovingCoroutine;

        private const string HitEventName = "Hit";
        
        public float Speed
        {
            set => _clip.Speed = value;
        }
        
        public override AnimationType AnimationType => AnimationType.Attack;

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

            _updatingMovingCoroutine = StartCoroutine(UpdatingMoving());
            
            _clip.Events.SetCallback(HitEventName, Hit);
            _clip.Events.OnEnd = _humanAnimations.StopIfNotAttacking;
        }

        public override void OnExitState()
        {
            base.OnExitState();

            if (_updatingMovingCoroutine != null)
            {
                StopCoroutine(_updatingMovingCoroutine);
                _updatingMovingCoroutine = null;
            }
        }

        private IEnumerator UpdatingMoving()
        {
            while (true)
            {
                UpdateMoving();

                yield return null;
            }
        }

        private void UpdateMoving()
        {
            if (_unitMeshAgent.IsMoving)
            {
                _humanAnimations.SetUpperBodyMaskForActions();
                
                if (!MovePlaying())
                {
                    _animancer.Layers[AnimationLayers.Main].Play(_moveClip);
                }
            }
            else
            {
                _humanAnimations.SetFullMaskForActions();
                
                if (!IdlePlaying())
                {
                    //_animancer.Layers[AnimationLayers.Main].Play(_idleClip);
                }
            }
        }

        private bool MovePlaying()
        {
            return _animancer.Layers[AnimationLayers.Main].IsPlayingClip(_moveClip.Clip);
        }

        private bool IdlePlaying()
        {
            return _animancer.Layers[AnimationLayers.Main].IsPlayingClip(_idleClip.Clip);
        }

        private void Hit()
        {
            _unitAttacker.Hit(GetHitTime());
        }

        private float GetHitTime()
        {
            return _clip.Events[HitEventName].normalizedTime * _clip.Clip.length;
        }
    }
}
