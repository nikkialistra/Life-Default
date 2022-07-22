using System;
using Entities;
using Pathfinding;
using ResourceManagement;
using Units;
using UnityEngine;

namespace Colonists
{
    [RequireComponent(typeof(ColonistMeshAgentPath))]
    [RequireComponent(typeof(ColonistAnimator))]
    [RequireComponent(typeof(UnitMeshAgent))]
    public class ColonistMeshAgent : MonoBehaviour
    {
        public event Action DestinationReach;
        public event Action RotationEnd;

        private bool _activated;
        private bool _hasPendingOrder;

        private ColonistAnimator _animator;
        private UnitMeshAgent _unitMeshAgent;

        private Seeker _seeker;

        private ColonistMeshAgentPath _meshAgentPath;

        private void Awake()
        {
            _animator = GetComponent<ColonistAnimator>();
            _unitMeshAgent = GetComponent<UnitMeshAgent>();
            _meshAgentPath = GetComponent<ColonistMeshAgentPath>();

            _seeker = GetComponent<Seeker>();
        }

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
            if (_unitMeshAgent.IsMoving)
                _animator.Move();
            else
                _animator.Idle();
        }

        public void SetDestinationToPosition(Vector3 position)
        {
            _hasPendingOrder = false;

            _unitMeshAgent.SetDestinationToPosition(position);
        }

        public void SetDestinationToResource(Resource resource, float atDistance)
        {
            _hasPendingOrder = false;

            _unitMeshAgent.SetDestinationToResource(resource, atDistance);
        }

        public bool CanAcceptOrder()
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
            if (_hasPendingOrder) return;

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

        public void ShowLinePath()
        {
            _seeker.pathCallback -= _meshAgentPath.ShowPathLine;
            _seeker.pathCallback += _meshAgentPath.ShowPathLine;
        }

        public void HideLinePath()
        {
            _seeker.pathCallback -= _meshAgentPath.ShowPathLine;
            _meshAgentPath.HidePathLine();
        }

        private void OnDestinationReach()
        {
            _meshAgentPath.HidePathLine();

            DestinationReach?.Invoke();
        }

        private void OnRotationEnd()
        {
            RotationEnd?.Invoke();
        }
    }
}
