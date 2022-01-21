using System;
using System.Collections;
using DG.Tweening;
using Entities.Entity;
using Pathfinding;
using UnityEngine;

namespace Units.Unit
{
    [RequireComponent(typeof(AIPath))]
    public class UnitMeshAgent : MonoBehaviour
    {
        [SerializeField] private float _rotationSpeedToEntities;
        [SerializeField] private float _interactionDistance = 3f;

        private bool _activated;
        private bool _hasPendingOrder;

        private bool _movingToEntity;

        private Coroutine _movingCoroutine;

        private AIPath _aiPath;

        private Coroutine _rotatingToCoroutine;

        private void Awake()
        {
            _aiPath = GetComponent<AIPath>();
        }

        public event Action DestinationReach;
        public event Action RotationEnd;

        public float Velocity => _aiPath.velocity.magnitude;

        public void SetDestinationToPosition(Vector3 position)
        {
            _movingToEntity = false;
            SetDestination(position);
        }

        public void SetDestinationToEntity(Vector3 position)
        {
            _movingToEntity = true;
            SetDestination(position);
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
            _rotatingToCoroutine = StartCoroutine(RotatingTo(entity));
        }

        public void StopMoving()
        {
            if (_hasPendingOrder)
            {
                return;
            }

            if (_movingCoroutine != null)
            {
                StopCoroutine(_movingCoroutine);
            }

            _aiPath.isStopped = true;
            DestinationReach?.Invoke();
        }

        public void StopRotating()
        {
            if (_rotatingToCoroutine != null)
            {
                StopCoroutine(_rotatingToCoroutine);
            }
        }

        private void SetDestination(Vector3 position)
        {
            _hasPendingOrder = false;

            _aiPath.isStopped = false;
            _aiPath.destination = position;
            Move();
        }

        private IEnumerator RotatingTo(Entity entity)
        {
            var targetPosition = entity.transform.position;
            targetPosition.y = transform.position.y;
            var targetDirection = (targetPosition - transform.position).normalized;
            var targetRotation = Quaternion.LookRotation(targetDirection).eulerAngles;

            yield return transform.DORotate(targetRotation, GetRotationDuration(targetRotation)).WaitForCompletion();

            RotationEnd?.Invoke();
        }

        private float GetRotationDuration(Vector3 targetRotation)
        {
            var angleDifference = GetAngleDifference(transform.rotation.eulerAngles.y, targetRotation.y);
            var duration = angleDifference / _rotationSpeedToEntities;
            return duration;
        }

        private float GetAngleDifference(float firstAngle, float secondAngle)
        {
            var difference = firstAngle - secondAngle;
            if (difference > 180)
            {
                difference -= 360;
            }

            if (difference < -180)
            {
                difference += 360;
            }

            return Mathf.Abs(difference);
        }

        public void Activate()
        {
            _activated = true;
        }

        public void Deactivate()
        {
            _aiPath.isStopped = true;
            _activated = false;
        }

        private void Move()
        {
            if (_movingCoroutine != null)
            {
                StopCoroutine(_movingCoroutine);
            }

            _movingCoroutine = StartCoroutine(Moving());
        }

        private IEnumerator Moving()
        {
            while (IsMoving())
            {
                yield return null;
            }

            DestinationReach?.Invoke();
        }

        private bool IsMoving()
        {
            return _movingToEntity ? IsMovingToEntity() : IsMovingToPosition();
        }

        private bool IsMovingToEntity()
        {
            if (Vector3.Distance(transform.position, _aiPath.destination) > _interactionDistance)
            {
                return true;
            }

            _aiPath.isStopped = true;

            return false;
        }

        private bool IsMovingToPosition()
        {
            if (!_aiPath.reachedDestination)
            {
                return true;
            }

            _aiPath.isStopped = true;

            return false;
        }
    }
}
