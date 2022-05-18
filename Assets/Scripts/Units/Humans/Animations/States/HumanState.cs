using System;
using System.Collections;
using Animancer;
using Animancer.FSM;
using Sirenix.OdinInspector;
using UnityEngine;

namespace Units.Humans.Animations.States
{
    [RequireComponent(typeof(AnimancerComponent))]
    [RequireComponent(typeof(HumanAnimations))]
    public abstract class HumanState : MonoBehaviour, IState
    {
        [SerializeField] protected ClipTransition _clip;
        
        [Space]
        [SerializeField] private bool _isAction;
        [Space]
        [ShowIf("_isAction")]
        [SerializeField] private UnitMeshAgent _unitMeshAgent;
        [ShowIf("_isAction")]
        [SerializeField] private float _waitTimeToIdle = 0.1f;

        protected HumanAnimations _humanAnimations;
        
        private AnimancerComponent _animancer;

        private Coroutine _lowerBodyOverwriteCoroutine;

        private bool _isMoving;

        private Coroutine _updatingMovingCoroutine;
        private Coroutine _idleCoroutine;

        private void Awake()
        {
            _humanAnimations = GetComponent<HumanAnimations>();
            
            _animancer = GetComponent<AnimancerComponent>();
        }

        public virtual AnimationType AnimationType =>
            throw new InvalidOperationException("Cannot get main animation type of base human state");

        public virtual bool CanEnterState => true;

        public virtual bool CanExitState => true;

        public virtual void OnEnterState()
        {
            _animancer.Layers[AnimationLayers.Main].Play(_clip);

            if (_isAction)
            {
                _updatingMovingCoroutine = StartCoroutine(UpdatingMoving());
            }
        }
        
        public virtual void OnExitState()
        {
            if (_isAction)
            {
                if (_updatingMovingCoroutine != null)
                {
                    StopCoroutine(_updatingMovingCoroutine);
                    _updatingMovingCoroutine = null;
                }
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
            _humanAnimations.LowerBodyOverwriteToMove();
        }

        private IEnumerator Idle()
        {
            yield return new WaitForSeconds(_waitTimeToIdle);
            
            _humanAnimations.LowerBodyOverwriteToIdle();
        }
    }
}
