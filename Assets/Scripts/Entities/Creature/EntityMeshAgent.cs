using System;
using System.Collections;
using DG.Tweening;
using Enemies;
using Pathfinding;
using ResourceManagement;
using UnityEngine;

namespace Entities.Creature
{
    public class EntityMeshAgent : MonoBehaviour
    {
        [SerializeField] private float _rotationSpeed = 120f;
        [SerializeField] private float _entityOffsetForPathRecalculation = 0.6f;

        private AIPath _aiPath;

        private float _interactionDistance;

        private bool _movingToEnemy;
        private Entity _entity;
        private Vector3 _lastEntityPosition;

        private bool _movingToResource;
        private Resource _resource;

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
        private bool IsRotating { get; set; }

        public void SetDestinationToPosition(Vector3 position)
        {
            _aiPath.isStopped = false;
            _aiPath.destination = (Vector3)AstarPath.active.GetNearest(position, NNConstraint.Default).node.position;

            Move();
        }

        public void SetDestinationToEnemy(Enemy enemy, float atDistance)
        {
            ResetDestination();

            if (_entity == enemy.Entity)
            {
                return;
            }

            _entity = enemy.Entity;
            _lastEntityPosition = _entity.transform.position;

            _interactionDistance = atDistance;
            _aiPath.isStopped = false;

            _aiPath.destination = _lastEntityPosition;

            _movingToEnemy = true;
            Move();
        }

        public void SetDestinationToResource(Resource resource, float atDistance)
        {
            ResetDestination();

            _resource = resource;

            _interactionDistance = atDistance;
            _aiPath.isStopped = false;

            _aiPath.destination = GetNearestWalkablePosition(resource.transform.position);

            _movingToResource = true;
            Move();
        }

        private void ResetDestination()
        {
            _movingToResource = false;
            _movingToEnemy = false;
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
            IsRotating = false;

            if (_rotatingToCoroutine != null)
            {
                StopCoroutine(_rotatingToCoroutine);
                _rotatingToCoroutine = null;
            }
        }

        public void RotateTo(Vector3 position)
        {
            IsRotating = true;

            if (_rotatingToCoroutine != null)
            {
                StopCoroutine(_rotatingToCoroutine);
            }

            _rotatingToCoroutine = StartCoroutine(RotatingTo(position));
        }

        public void RotateToAngle(float angle)
        {
            IsRotating = true;

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
            if (_movingToEnemy)
            {
                return UpdateMovingToEnemy();
            }

            if (_movingToResource)
            {
                return UpdateMovingToResource();
            }

            return UpdateMovingToPosition();
        }

        private bool UpdateMovingToEnemy()
        {
            if (Vector3.Distance(transform.position, _aiPath.destination) <= _interactionDistance)
            {
                _entity = null;
                return false;
            }

            if (Vector3.Distance(_entity.transform.position, _lastEntityPosition) > _entityOffsetForPathRecalculation)
            {
                _lastEntityPosition = _entity.transform.position;
                _aiPath.destination = _lastEntityPosition;
            }

            return true;
        }

        private bool UpdateMovingToResource()
        {
            if (Vector3.Distance(transform.position, _aiPath.destination) <= _interactionDistance)
            {
                _resource = null;
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

            IsRotating = false;
            RotationEnd?.Invoke();
        }

        private IEnumerator RotatingToAngle(float angle)
        {
            var targetRotation = new Vector3(0, angle, 0);

            yield return transform.DORotate(targetRotation, GetRotationDuration(targetRotation)).WaitForCompletion();

            IsRotating = false;
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
