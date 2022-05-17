using System.Collections;
using Animancer;
using Animancer.FSM;
using Sirenix.OdinInspector;
using Units.Humans.Animations.Actions;
using Units.Humans.Animations.Actions.States;
using Units.Humans.Animations.Main;
using Units.Humans.Animations.Main.States;
using UnityEngine;

namespace Units.Humans.Animations.States
{
    [EventNames(HitEvent, HitEndEvent)]
    public class AttackState : ActionsHumanState
    {
        [SerializeField] private float _timeToFade = 0.1f;

        [Title("Components")]
        [Required]
        [SerializeField] private UnitAttacker _unitAttacker;
        [Required]
        [SerializeField] private UnitMeshAgent _unitMeshAgent;
        [Required]
        [SerializeField] private HumanAnimations _humanAnimations;
      
        [Title("Additional")]
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

        private void OnEnable()
        {
            _clip.Events.SetCallback(HitEvent, Hit);
            _clip.Events.SetCallback(HitEndEvent, _humanAnimations.StopIfNotAttacking);
        }

        private void OnDisable()
        {
            _clip.Events.RemoveCallback(HitEvent, Hit);
            _clip.Events.RemoveCallback(HitEndEvent, _humanAnimations.StopIfNotAttacking);
        }

        public float Speed
        {
            set => _clip.Speed = value;
        }
        
        public override ActionsAnimationType ActionsAnimationType => ActionsAnimationType.Attack;

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

        public override void OnEnterState()
        {
            base.OnEnterState();

            _updatingMovingCoroutine = StartCoroutine(UpdatingMoving());
        }

        public override void OnExitState()
        {
            if (_updatingMovingCoroutine != null)
            {
                StopCoroutine(_updatingMovingCoroutine);
                _updatingMovingCoroutine = null;
            }
            
            _animancer.Layers[AnimationLayers.Actions].StartFade(0, _timeToFade);
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
