using System;
using System.Collections;
using DG.Tweening;
using Infrastructure.Settings;
using Pathfinding;
using ResourceManagement;
using Units.Stats;
using UnityEngine;
using Zenject;

namespace Units
{
    [RequireComponent(typeof(AIPath))]
    [RequireComponent(typeof(Seeker))]
    public class UnitMeshAgent : MonoBehaviour
    {
        public event Action DestinationReach;
        public event Action RotationEnd;

        public bool IsMoving => !_aiPath.isStopped;

        [SerializeField] private float _rotationSpeed = 120f;
        [SerializeField] private float _entityOffsetForPathRecalculation = 0.6f;
        [Space]
        [SerializeField] private float _minMoveTime = 0.3f;
        [SerializeField] private float _velocityToStop = 0.15f;

        private AIPath _aiPath;

        private float _interactionDistance;

        private bool _movingToUnitTarget;
        private Unit _unitTarget;
        private Vector3 _lastUnitTargetPosition;

        private float _seekPredictionMultiplier;

        private bool _movingToResource;

        private float _timeSetToDestination;

        private Coroutine _movingCoroutine;
        private Coroutine _rotatingToCoroutine;

        [Inject]
        public void Construct(AttackSettings attackSettings)
        {
            _seekPredictionMultiplier = attackSettings.SeekPredictionMultiplier;
        }

        private void Awake()
        {
            _aiPath = GetComponent<AIPath>();
            _aiPath.isStopped = true;
        }

        public void BindStats(Stat<UnitStat> movementSpeed)
        {
            _aiPath.maxSpeed = movementSpeed.Value;
            movementSpeed.ValueChange += ChangeMovementSpeed;
        }

        public void UnbindStats(Stat<UnitStat> movementSpeed)
        {
            movementSpeed.ValueChange -= ChangeMovementSpeed;
        }

        public void SetDestinationToPosition(Vector3 position)
        {
            _timeSetToDestination = Time.time;

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

            _aiPath.destination = _lastUnitTargetPosition + CalculateForwardCorrection();

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

        private void ChangeMovementSpeed(float value)
        {
            if (value < 0) throw new ArgumentException("Movement speed cannot be less than 0");

            _aiPath.maxSpeed = value;
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
            if (_rotatingToCoroutine != null) return;

            _rotatingToCoroutine = StartCoroutine(CRotatingTo(position));
        }

        public void RotateToAngle(float angle)
        {
            if (_rotatingToCoroutine != null) return;

            _rotatingToCoroutine = StartCoroutine(CRotatingToAngle(angle));
        }

        public float GetAngleDifferenceWith(Vector3 targetPosition)
        {
            var targetRotation = Quaternion.LookRotation(targetPosition - transform.position).eulerAngles;

            return GetAngleDifference(transform.rotation.eulerAngles.y, targetRotation.y);
        }

        public void StopMovingToUnitTarget()
        {
            _unitTarget = null;
        }

        private void Move()
        {
            if (_movingCoroutine != null)
                StopCoroutine(_movingCoroutine);

            _movingCoroutine = StartCoroutine(CMoving());
        }

        private IEnumerator CMoving()
        {
            while (UpdateMoving())
                yield return null;

            _aiPath.isStopped = true;
            DestinationReach?.Invoke();
        }

        private bool UpdateMoving()
        {
            if (_movingToUnitTarget)
                return IsMovingToUnitTarget();

            if (_movingToResource)
                return IsMovingToResource();

            return IsMovingToPosition();
        }

        private bool IsMovingToUnitTarget()
        {
            if (_unitTarget == null)
            {
                _movingToUnitTarget = false;
                return false;
            }

            if (Vector3.Distance(transform.position, _unitTarget.transform.position) <= _interactionDistance)
            {
                _unitTarget = null;
                _movingToUnitTarget = false;
                return false;
            }

            if (Vector3.Distance(_unitTarget.transform.position, _lastUnitTargetPosition) >
                _entityOffsetForPathRecalculation)
            {
                _lastUnitTargetPosition = _unitTarget.transform.position;
                _aiPath.destination = _lastUnitTargetPosition + CalculateForwardCorrection();
            }

            return true;
        }

        private Vector3 CalculateForwardCorrection()
        {
            return (_lastUnitTargetPosition - transform.position).normalized * _seekPredictionMultiplier;
        }

        private bool IsMovingToResource()
        {
            if (Vector3.Distance(transform.position, _aiPath.destination) <= _interactionDistance)
                return false;

            return true;
        }

        private bool IsMovingToPosition()
        {
            if (Time.time - _timeSetToDestination < _minMoveTime)
                return true;

            return _aiPath.velocity.magnitude > _velocityToStop && !_aiPath.reachedDestination;
        }

        private IEnumerator CRotatingTo(Vector3 targetPosition)
        {
            var targetRotation = Quaternion.LookRotation(targetPosition - transform.position).eulerAngles;

            yield return transform.DORotate(targetRotation, GetRotationDuration(targetRotation)).WaitForCompletion();

            _rotatingToCoroutine = null;
            RotationEnd?.Invoke();
        }

        private IEnumerator CRotatingToAngle(float angle)
        {
            var targetRotation = new Vector3(0, angle, 0);

            yield return transform.DORotate(targetRotation, GetRotationDuration(targetRotation)).WaitForCompletion();

            _rotatingToCoroutine = null;
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
                difference -= 360;

            if (difference < -180)
                difference += 360;

            return Mathf.Abs(difference);
        }
    }
}
