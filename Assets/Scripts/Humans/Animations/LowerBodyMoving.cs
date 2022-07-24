using System.Collections;
using CompositionRoot.Settings;
using Humans.Animations.States;
using Units;
using UnityEngine;

namespace Humans.Animations
{
    public class LowerBodyMoving
    {
        private readonly HumanState _humanState;
        private readonly UnitMeshAgent _unitMeshAgent;
        private readonly HumanAnimations _humanAnimations;

        private readonly float _waitTimeToIdle;

        private Coroutine _lowerBodyOverwriteCoroutine;

        private bool _isMoving;

        private Coroutine _updatingMovingCoroutine;
        private Coroutine _idleCoroutine;

        public LowerBodyMoving(HumanState humanState, UnitMeshAgent unitMeshAgent, HumanAnimations humanAnimations,
            AnimationSettings animationSettings)
        {
            _humanState = humanState;
            _unitMeshAgent = unitMeshAgent;
            _humanAnimations = humanAnimations;

            _waitTimeToIdle = animationSettings.WaitTimeToIdle;
        }

        public void Start()
        {
            _updatingMovingCoroutine = _humanState.StartCoroutine(CUpdatingMoving());
        }

        public void Stop()
        {
            _humanAnimations.LowerBodyOverwriteToIdle();

            if (_updatingMovingCoroutine != null)
            {
                _humanState.StopCoroutine(_updatingMovingCoroutine);
                _updatingMovingCoroutine = null;
            }
        }

        private IEnumerator CUpdatingMoving()
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
            if (_isMoving == _unitMeshAgent.IsMoving) return;

            _isMoving = _unitMeshAgent.IsMoving;
            UpdateBaseAnimation();
        }

        private void UpdateBaseAnimation()
        {
            if (_isMoving)
            {
                if (_idleCoroutine != null)
                {
                    _humanState.StopCoroutine(_idleCoroutine);
                    _idleCoroutine = null;
                }

                Move();
            }
            else
            {
                _idleCoroutine = _humanState.StartCoroutine(CIdle());
            }
        }

        private void Move()
        {
            _humanAnimations.LowerBodyOverwriteToMove();
        }

        private IEnumerator CIdle()
        {
            yield return new WaitForSeconds(_waitTimeToIdle);

            _humanAnimations.LowerBodyOverwriteToIdle();
        }
    }
}
