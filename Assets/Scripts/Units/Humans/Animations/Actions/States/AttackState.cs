using System.Collections;
using Animancer;
using Sirenix.OdinInspector;
using UnityEngine;

namespace Units.Humans.Animations.Actions.States
{
    [EventNames(HitEvent, HitEndEvent)]
    public class AttackState : ActionsHumanState
    {
        [Title("Components")]
        [Required]
        [SerializeField] private UnitAttacker _unitAttacker;
        [Required]
        [SerializeField] private UnitMeshAgent _unitMeshAgent;
        [Required]
        [SerializeField] private HumanAnimations _humanAnimations;
      
        [Title("Additional")]
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
        }

        private IEnumerator UpdatingMoving()
        {
            _isMoving = _unitMeshAgent.IsMoving;
            UpdateBaseAnimation();

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
        }

        private IEnumerator Idle()
        {
            yield return new WaitForSeconds(_waitTimeToIdle);
            
            _humanAnimations.SetFullMaskForActions();
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
