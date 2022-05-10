using System;
using Entities;
using Pathfinding;
using ResourceManagement;
using Sirenix.OdinInspector;
using Units;
using UnityEngine;
using Zenject;

namespace Colonists
{
    [RequireComponent(typeof(ColonistAnimator))]
    [RequireComponent(typeof(UnitMeshAgent))]
    public class ColonistMeshAgent : MonoBehaviour
    {
        [Required]
        [SerializeField] private LineRenderer _pathLineRenderer;
        [SerializeField] private Vector3 _pathLineOffset = new(0, 0.2f, 0);

        private bool _activated;
        private bool _hasPendingOrder;

        private ColonistAnimator _animator;
        private UnitMeshAgent _unitMeshAgent;
        
        private Seeker _seeker;
        
        private Transform _pathLineParent;

        [Inject]
        public void Construct(Transform pathLineParent)
        {
            _pathLineParent = pathLineParent;
        }

        private void Awake()
        {
            _animator = GetComponent<ColonistAnimator>();
            _unitMeshAgent = GetComponent<UnitMeshAgent>();
            
            _seeker = GetComponent<Seeker>();
        }

        public event Action DestinationReach;
        public event Action RotationEnd;

        private void Start()
        {
            _pathLineRenderer.transform.parent = _pathLineParent;
            _pathLineRenderer.transform.position = Vector3.zero;
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
            {
                _animator.Move();
            }
            else
            {
                _animator.Idle();
            }
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

        public void ShowLinePath()
        {
            _seeker.pathCallback -= ShowPathLine;
            _seeker.pathCallback += ShowPathLine;
        }

        public void HideLinePath()
        {
            _seeker.pathCallback -= ShowPathLine;
            _pathLineRenderer.positionCount = 0;
        }

        private void ShowPathLine(Path path)
        {
            var pathNodes = path.path;
            
            if (pathNodes.Count <= 2)
            {
                _pathLineRenderer.positionCount = 0;
                return;
            }

            _pathLineRenderer.positionCount = pathNodes.Count - 1;

            for (int i = 1; i < pathNodes.Count; i++)
            {
                _pathLineRenderer.SetPosition(i - 1,
                    (Vector3)pathNodes[i].position + _pathLineOffset);
            }
        }

        private void OnDestinationReach()
        {
            HideLinePath();
            
            DestinationReach?.Invoke();
        }

        private void OnRotationEnd()
        {
            RotationEnd?.Invoke();
        }
    }
}
