using System;
using Enemies;
using Entities;
using ResourceManagement;
using Units;
using UnityEngine;

namespace Colonists
{
    [RequireComponent(typeof(ColonistAnimator))]
    [RequireComponent(typeof(UnitMeshAgent))]
    public class ColonistMeshAgent : MonoBehaviour
    {
        private bool _activated;
        private bool _hasPendingOrder;

        private ColonistAnimator _animator;
        private UnitMeshAgent _unitMeshAgent;

        private void Awake()
        {
            _animator = GetComponent<ColonistAnimator>();
            _unitMeshAgent = GetComponent<UnitMeshAgent>();
        }

        public event Action DestinationReach;
        public event Action RotationEnd;

        private void OnEnable()
        {
            _unitMeshAgent.DestinationReach += OnDestinationReach;
            _unitMeshAgent.RotationEnd += OnRotationEnd;
        }

        private void OnDisable()
        {
            _unitMeshAgent.DestinationReach -= OnDestinationReach;
            _unitMeshAgent.RotationEnd -= OnRotationEnd;
        }

        private void Update()
        {
            _animator.Move(_unitMeshAgent.IsMoving);
        }

        public void SetDestinationToPosition(Vector3 position)
        {
            _hasPendingOrder = false;

            _unitMeshAgent.SetDestinationToPosition(position);
        }

        public void SetDestinationToUnitTarget(Unit unitTarget, float atDistance)
        {
            _hasPendingOrder = false;

            _unitMeshAgent.SetDestinationToUnitTarget(unitTarget, atDistance);
        }

        public void SetDestinationToResource(Resource resource, float atDistance)
        {
            _hasPendingOrder = false;

            _unitMeshAgent.SetDestinationToResource(resource, atDistance);
        }

        public bool AcceptOrder()
        {
            if (!_activated)
            {
                return false;
            }
            else
            {
                _hasPendingOrder = true;
                return true;
            }
        }

        public void RotateTo(Entity entity)
        {
            _unitMeshAgent.RotateTo(entity.transform.position);
        }

        public void RotateToAngle(float angle)
        {
            _unitMeshAgent.RotateToAngle(angle);
        }

        public void StopMoving()
        {
            if (_hasPendingOrder)
            {
                return;
            }

            _unitMeshAgent.StopMoving();
        }

        public void StopRotating()
        {
            _unitMeshAgent.StopRotating();
        }

        public void Activate()
        {
            _activated = true;
        }

        public void Deactivate()
        {
            _activated = false;

            _unitMeshAgent.StopCurrentCommand();
        }

        public void ResetDestination()
        {
           _unitMeshAgent.ResetDestination();
        }

        private void OnDestinationReach()
        {
            DestinationReach?.Invoke();
        }

        private void OnRotationEnd()
        {
            RotationEnd?.Invoke();
        }
    }
}
