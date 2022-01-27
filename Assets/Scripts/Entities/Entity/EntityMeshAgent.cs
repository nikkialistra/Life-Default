using System;
using System.Collections;
using DG.Tweening;
using Pathfinding;
using UnityEngine;

namespace Entities.Entity
{
    public class EntityMeshAgent : MonoBehaviour
    {
        [SerializeField] private float _rotationSpeed = 120;
        [SerializeField] private float _interactionDistance = 3f;

        private AIPath _aiPath;

        private bool _movingToEntity;

        private Coroutine _movingCoroutine;
        private Coroutine _rotatingToCoroutine;

        private void Awake()
        {
            _aiPath = GetComponent<AIPath>();
        }

        private void Start()
        {
            _aiPath.isStopped = true;
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

        public void StopMoving()
        {
            if (_movingCoroutine != null)
            {
                StopCoroutine(_movingCoroutine);
            }

            _aiPath.isStopped = true;
            DestinationReach?.Invoke();
        }

        public void StopCurrentCommand()
        {
            _aiPath.isStopped = true;
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
            _aiPath.isStopped = false;
            _aiPath.destination = position;
            Move();
        }

        public void RotateTo(Vector3 position)
        {
            if (_rotatingToCoroutine != null)
            {
                StopCoroutine(_rotatingToCoroutine);
            }

            _rotatingToCoroutine = StartCoroutine(RotatingTo(position));
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

            _aiPath.isStopped = true;
            DestinationReach?.Invoke();
        }

        private bool IsMoving()
        {
            return _movingToEntity ? IsMovingToEntity() : IsMovingToPosition();
        }

        private bool IsMovingToEntity()
        {
            return Vector3.Distance(transform.position, _aiPath.destination) > _interactionDistance;
        }

        private bool IsMovingToPosition()
        {
            return !_aiPath.reachedDestination;
        }

        private IEnumerator RotatingTo(Vector3 targetPosition)
        {
            targetPosition.y = transform.position.y;
            var targetDirection = (targetPosition - transform.position).normalized;

            if (targetDirection == Vector3.zero)
            {
                RotationEnd?.Invoke();
                yield break;
            }

            var targetRotation = Quaternion.LookRotation(targetDirection).eulerAngles;

            yield return transform.DORotate(targetRotation, GetRotationDuration(targetRotation)).WaitForCompletion();

            RotationEnd?.Invoke();
        }

        private float GetRotationDuration(Vector3 targetRotation)
        {
            var angleDifference = GetAngleDifference(transform.rotation.eulerAngles.y, targetRotation.y);
            var duration = angleDifference / _rotationSpeed;
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
    }
}
