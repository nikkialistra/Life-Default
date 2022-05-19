﻿using System.Collections;
using General;
using Units.Humans.Animations.States;
using UnityEngine;

namespace Units.Humans.Animations
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

        public LowerBodyMoving(HumanState humanState, UnitMeshAgent unitMeshAgent, HumanAnimations humanAnimations)
        {
            _humanState = humanState;
            _unitMeshAgent = unitMeshAgent;
            _humanAnimations = humanAnimations;
            
            _waitTimeToIdle = GlobalParameters.Instance.WaitTimeToIdle;
        }

        public void Start()
        {
            _updatingMovingCoroutine = _humanState.StartCoroutine(UpdatingMoving());
        }

        public void Stop()
        {
            if (_updatingMovingCoroutine != null)
            {
                _humanState.StopCoroutine(_updatingMovingCoroutine);
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
                    _humanState.StopCoroutine(_idleCoroutine);
                    _idleCoroutine = null;
                }
                
                Move();
            }
            else
            {
                _idleCoroutine = _humanState.StartCoroutine(Idle());
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