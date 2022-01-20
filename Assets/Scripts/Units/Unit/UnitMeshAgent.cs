using System;
using System.Collections;
using DG.Tweening;
using Entities.Entity;
using Pathfinding;
using UnityEngine;

namespace Units.Unit
{
    [RequireComponent(typeof(UnitFacade))]
    [RequireComponent(typeof(AIPath))]
    public class UnitMeshAgent : MonoBehaviour
    {
        [SerializeField] private float _rotationSpeedToEntities;

        private bool _activated;

        private Coroutine _movingCoroutine;

        private UnitFacade _unitFacade;
        private AIPath _aiPath;

        private void Awake()
        {
            _unitFacade = GetComponent<UnitFacade>();
            _aiPath = GetComponent<AIPath>();
        }

        public event Action DestinationReach;
        public event Action RotationEnd;

        public float Velocity => _aiPath.velocity.magnitude;

        public bool CanAcceptOrder => _activated;

        private void OnEnable()
        {
            _unitFacade.Spawn += Activate;
            _unitFacade.Die += Deactivate;
        }

        private void OnDisable()
        {
            _unitFacade.Spawn -= Activate;
            _unitFacade.Die -= Deactivate;
        }

        public void SetDestination(Vector3 position)
        {
            _aiPath.isStopped = false;
            _aiPath.destination = position;
            Move();
        }

        public void RotateTo(Entity entity)
        {
            StartCoroutine(RotatingTo(entity));
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

        private void Activate()
        {
            _aiPath.isStopped = false;
            _activated = true;
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
            if (_aiPath.reachedDestination)
            {
                _aiPath.isStopped = true;
            }

            return !_aiPath.reachedDestination;
        }

        private void Deactivate()
        {
            _aiPath.isStopped = true;
            _activated = false;
        }
    }
}
