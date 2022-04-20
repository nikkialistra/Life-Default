using System;
using Enemies;
using Entities;
using ResourceManagement;
using Units;
using UnityEngine;

namespace Colonists
{
    [RequireComponent(typeof(ColonistAnimator))]
    [RequireComponent(typeof(EntityMeshAgent))]
    public class ColonistMeshAgent : MonoBehaviour
    {
        private bool _activated;
        private bool _hasPendingOrder;

        private ColonistAnimator _animator;
        private EntityMeshAgent _entityMeshAgent;

        private void Awake()
        {
            _animator = GetComponent<ColonistAnimator>();
            _entityMeshAgent = GetComponent<EntityMeshAgent>();
        }

        public event Action DestinationReach;
        public event Action RotationEnd;

        private void OnEnable()
        {
            _entityMeshAgent.DestinationReach += OnDestinationReach;
            _entityMeshAgent.RotationEnd += OnRotationEnd;
        }

        private void OnDisable()
        {
            _entityMeshAgent.DestinationReach -= OnDestinationReach;
            _entityMeshAgent.RotationEnd -= OnRotationEnd;
        }

        private void Update()
        {
            _animator.Move(_entityMeshAgent.IsMoving);
        }

        public void SetDestinationToPosition(Vector3 position)
        {
            _hasPendingOrder = false;

            _entityMeshAgent.SetDestinationToPosition(position);
        }

        public void SetDestinationToEnemy(Enemy enemy, float atDistance)
        {
            _hasPendingOrder = false;

            _entityMeshAgent.SetDestinationToEnemy(enemy, atDistance);
        }

        public void SetDestinationToResource(Resource resource, float atDistance)
        {
            _hasPendingOrder = false;

            _entityMeshAgent.SetDestinationToResource(resource, atDistance);
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
            _entityMeshAgent.RotateTo(entity.transform.position);
        }

        public void RotateToAngle(float angle)
        {
            _entityMeshAgent.RotateToAngle(angle);
        }

        public void StopMoving()
        {
            if (_hasPendingOrder)
            {
                return;
            }

            _entityMeshAgent.StopMoving();
        }

        public void StopRotating()
        {
            _entityMeshAgent.StopRotating();
        }

        public void Activate()
        {
            _activated = true;
        }

        public void Deactivate()
        {
            _activated = false;

            _entityMeshAgent.StopCurrentCommand();
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
