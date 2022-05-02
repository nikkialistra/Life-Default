using System;
using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using Pathfinding;
using ResourceManagement;
using Sirenix.OdinInspector;
using UnityEngine;

namespace Units
{
    [RequireComponent(typeof(AIPath))]
    [RequireComponent(typeof(Seeker))]
    public class UnitMeshAgent : MonoBehaviour
    {
        [SerializeField] private float _rotationSpeed = 120f;
        [SerializeField] private float _entityOffsetForPathRecalculation = 0.6f;

        private AIPath _aiPath;

        private float _interactionDistance;

        private bool _movingToUnitTarget;
        private Unit _unitTarget;
        private Vector3 _lastUnitTargetPosition;

        private bool _movingToResource;

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

        public bool IsMoving => !_aiPath.isStopped;

        public void SetDestinationToPosition(Vector3 position)
        {
            _aiPath.isStopped = false;
            _aiPath.destination = (Vector3)AstarPath.active.GetNearest(position, NNConstraint.Default).node.position;
            
            Move();
        }

        public void SetDestinationToUnitTarget(Unit unitTarget, float atDistance)
        {
            ResetDestination();

            _unitTarget = unitTarget;
            _lastUnitTargetPosition = _unitTarget.transform.position;

            _interactionDistance = atDistance;
            _aiPath.isStopped = false;

            _aiPath.destination = _lastUnitTargetPosition;

            _movingToUnitTarget = true;
            Move();
        }

        public void SetDestinationToResource(Resource resource, float atDistance)
        {
            ResetDestination();

            _interactionDistance = atDistance;
            _aiPath.isStopped = false;

            _aiPath.destination = GetNearestWalkablePosition(resource.transform.position);

            _movingToResource = true;
            Move();
        }

        public void ResetDestination()
        {
            _movingToResource = false;
            _movingToUnitTarget = false;
            
            StopRotating();
        }

        private Vector3 GetNearestWalkablePosition(Vector3 originalPosition)
        {
            var nearestPosition = Vector3.negativeInfinity;
            var minDistance = float.PositiveInfinity;

            var activeGraph = AstarPath.active;
            var sidePositions = new[]
            {
                originalPosition + Vector3.left,
                originalPosition + Vector3.left + Vector3.forward,
                originalPosition + Vector3.forward,
                originalPosition + Vector3.right + Vector3.forward,
                originalPosition + Vector3.right,
                originalPosition + Vector3.right + Vector3.back,
                originalPosition + Vector3.back,
                originalPosition + Vector3.back + Vector3.left
            };

            foreach (var sidePosition in sidePositions)
            {
                var position = (Vector3)activeGraph.GetNearest(sidePosition, NNConstraint.Default).node.position;
                var distance = Vector3.Distance(transform.position, position);

                if (distance < minDistance)
                {
                    nearestPosition = position;
                    minDistance = distance;
                }
            }

            return nearestPosition;
        }

        public void StopMoving()
        {
            if (_movingCoroutine != null)
            {
                StopCoroutine(_movingCoroutine);
                _movingCoroutine = null;
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
                _rotatingToCoroutine = null;
            }
        }

        public void RotateTo(Vector3 position)
        {
            if (_rotatingToCoroutine != null)
            {
                StopCoroutine(_rotatingToCoroutine);
            }

            _rotatingToCoroutine = StartCoroutine(RotatingTo(position));
        }

        public void RotateToAngle(float angle)
        {
            if (_rotatingToCoroutine != null)
            {
                StopCoroutine(_rotatingToCoroutine);
            }

            _rotatingToCoroutine = StartCoroutine(RotatingToAngle(angle));
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
            while (UpdateMoving())
            {
                yield return null;
            }

            _aiPath.isStopped = true;
            DestinationReach?.Invoke();
        }

        private bool UpdateMoving()
        {
            if (_movingToUnitTarget)
            {
                return UpdateMovingToUnitTarget();
            }

            if (_movingToResource)
            {
                return UpdateMovingToResource();
            }

            return UpdateMovingToPosition();
        }

        private bool UpdateMovingToUnitTarget()
        {
            if (Vector3.Distance(transform.position, _aiPath.destination) <= _interactionDistance)
            {
                _unitTarget = null;
                return false;
            }

            if (Vector3.Distance(_unitTarget.transform.position, _lastUnitTargetPosition) > _entityOffsetForPathRecalculation)
            {
                _lastUnitTargetPosition = _unitTarget.transform.position;
                _aiPath.destination = _lastUnitTargetPosition;
            }

            return true;
        }

        private bool UpdateMovingToResource()
        {
            if (Vector3.Distance(transform.position, _aiPath.destination) <= _interactionDistance)
            {
                return false;
            }

            return true;
        }

        private bool UpdateMovingToPosition()
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

        private IEnumerator RotatingToAngle(float angle)
        {
            var targetRotation = new Vector3(0, angle, 0);

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
