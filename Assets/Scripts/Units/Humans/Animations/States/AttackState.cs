using System.Collections;
using Animancer;
using Animancer.FSM;
using Sirenix.OdinInspector;
using UnityEngine;

namespace Units.Humans.Animations.States
{
    [EventNames(HitEvent, HitEndEvent)]
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
        [Space]
        [SerializeField] private float _waitTimeToIdle = 0.1f;

        private const string HitEvent = "Hit";
        private const string HitEndEvent = "Hit End";

        private Coroutine _updatingMovingCoroutine;

        private bool _isMoving;
        
        private Coroutine _idleCoroutine;

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
            
            _clip.Events.SetCallback(HitEvent, Hit);
            _clip.Events.SetCallback(HitEndEvent, _humanAnimations.StopIfNotAttacking);
        }

        public override void OnExitState()
        {
            _clip.Events.RemoveCallback(HitEvent, Hit);
            _clip.Events.RemoveCallback(HitEndEvent, _humanAnimations.StopIfNotAttacking);
            
            if (_updatingMovingCoroutine != null)
            {
                StopCoroutine(_updatingMovingCoroutine);
                _updatingMovingCoroutine = null;
            }
            
            base.OnExitState();
        }

        private IEnumerator UpdatingMoving()
        {
            _isMoving = _unitMeshAgent.IsMoving;
            
            while (true)
            {
                UpdateMoving();

                yield return null;
            }
        }

        private void UpdateMoving()
        {
            if (_isMoving == _unitMeshAgent.IsMoving)
            {
                return;
            }

            _isMoving = _unitMeshAgent.IsMoving;
            UpdateBaseAnimation();
        }

        private void UpdateBaseAnimation()
        {
            if (_isMoving)
            {
                if (_idleCoroutine != null)
                {
                    StopCoroutine(_idleCoroutine);
                    _idleCoroutine = null;
                }
                
                Move();
            }
            else
            {
                _idleCoroutine = StartCoroutine(Idle());
            }
        }

        private void Move()
        {
            _humanAnimations.SetUpperBodyMaskForActions();

            if (!MovePlaying())
            {
                _animancer.Layers[AnimationLayers.Main].Play(_moveClip);
            }
        }

        private IEnumerator Idle()
        {
            yield return new WaitForSeconds(_waitTimeToIdle);
            
            _humanAnimations.SetFullMaskForActions();

            if (!IdlePlaying())
            {
                _animancer.Layers[AnimationLayers.Main].Play(_idleClip);
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
            return _clip.Events[HitEvent].normalizedTime * _clip.Clip.length;
        }
    }
}
