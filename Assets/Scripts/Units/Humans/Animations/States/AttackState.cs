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
        
        [Space]
        [SerializeField] private ClipTransition _moveClip;
        
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
                _animancer.Play(_moveClip);
            }
            else
            {
                _animancer.Stop(_moveClip);
            }
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
