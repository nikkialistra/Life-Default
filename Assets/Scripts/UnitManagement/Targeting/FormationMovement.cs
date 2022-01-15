using System;
using System.Collections.Generic;
using UnityEngine;
using Zenject;

namespace UnitManagement.Targeting
{
    [RequireComponent(typeof(AreaFormation))]
    public class FormationMovement : MonoBehaviour
    {
        [SerializeField] private FormationType _formationType;

        private List<ITargetable> _targetables;
        private Target _target;

        private TargetPool _targetPool;

        private AreaFormation _areaFormation;

        [Inject]
        public void Construct(TargetPool targetPool)
        {
            _targetPool = targetPool;
        }

        private void Awake()
        {
            _areaFormation = GetComponent<AreaFormation>();
        }

        public void MoveTo(List<ITargetable> targetables, Target target)
        {
            _targetables = targetables;
            _target = target;

            MoveTargetablesToPositions(GenerateFormation(target.transform.position));
        }

        private void MoveTargetablesToPositions(Vector3[] formationPositions)
        {
            var assignedPositionsBitmask = new bool[formationPositions.Length];
            var assignedTargetablesBitmask = new bool[formationPositions.Length];

            var middlePoint = FindMiddlePointBetweenTargetables();

            for (var i = 0; i < formationPositions.Length; i++)
            {
                var formationIndex = _formationType == FormationType.Free
                    ? FarthestPointIndexFrom(formationPositions, assignedPositionsBitmask, middlePoint)
                    : i;

                var closestTargetableIndex =
                    ClosestTargetableIndexTo(formationPositions[formationIndex], assignedTargetablesBitmask);

                assignedPositionsBitmask[formationIndex] = true;
                assignedTargetablesBitmask[closestTargetableIndex] = true;

                var accepted = _targetables[closestTargetableIndex]
                    .TryAcceptTargetPoint(formationPositions[formationIndex]);
                if (accepted)
                {
                    _targetPool.Link(_target, _targetables[closestTargetableIndex]);
                }
            }
        }

        private static int FarthestPointIndexFrom(Vector3[] formationPositions, bool[] assignedPositionsBitmask,
            Vector3 originPoint)
        {
            var farthestPointDistance = 0f;
            var farthestPointIndex = 0;
            for (var i = 0; i < assignedPositionsBitmask.Length; i++)
            {
                if (assignedPositionsBitmask[i])
                {
                    continue;
                }

                var distanceToPoint = Vector3.Distance(originPoint, formationPositions[i]);
                if (distanceToPoint > farthestPointDistance)
                {
                    farthestPointDistance = distanceToPoint;
                    farthestPointIndex = i;
                }
            }

            return farthestPointIndex;
        }

        private int ClosestTargetableIndexTo(Vector3 targetPoint, bool[] assignedTargetablesBitmask)
        {
            var closestTargetableDistance = 1000f;
            float distanceToPoint;
            var closestTargetableIndex = 0;
            for (var i = 0; i < assignedTargetablesBitmask.Length; i++)
            {
                if (assignedTargetablesBitmask[i])
                {
                    continue;
                }

                distanceToPoint = Vector3.Distance(_targetables[i].Position, targetPoint);
                if (distanceToPoint < closestTargetableDistance)
                {
                    closestTargetableDistance = distanceToPoint;
                    closestTargetableIndex = i;
                }
            }

            return closestTargetableIndex;
        }

        private Vector3[] GenerateFormation(Vector3 targetPoint)
        {
            if (_targetables.Count == 0)
            {
                return Array.Empty<Vector3>();
            }

            return _formationType switch
            {
                FormationType.Free => GenerateFreeFormation(targetPoint),
                FormationType.Facing => GenerateFacingFormation(targetPoint),
                FormationType.Square => GenerateSquareFormation(targetPoint),
                FormationType.Diamond => GenerateDiamondFormation(targetPoint),
                FormationType.None => GenerateNoFormation(targetPoint),
                _ => throw new ArgumentOutOfRangeException()
            };
        }

        private Vector3[] GenerateFreeFormation(Vector3 targetPoint)
        {
            var originPoint = FindMiddlePointBetweenTargetables();

            var difference = targetPoint - originPoint;

            var freeFormationPositions = new Vector3[_targetables.Count];
            for (var i = 0; i < _targetables.Count; i++)
            {
                var targetPosition = _targetables[i].Position + difference;
                freeFormationPositions[i] = new Vector3(
                    targetPosition.x, targetPoint.y, targetPosition.z
                );
            }

            return freeFormationPositions;
        }

        private Vector3[] GenerateFacingFormation(Vector3 targetPoint)
        {
            return GenerateAreaFormation(targetPoint, true);
        }

        private Vector3[] GenerateSquareFormation(Vector3 targetPoint)
        {
            return GenerateAreaFormation(targetPoint);
        }

        private Vector3[] GenerateDiamondFormation(Vector3 targetPoint)
        {
            return GenerateAreaFormation(targetPoint, false, 45f);
        }

        private Vector3[] GenerateAreaFormation(Vector3 targetPoint, bool faceTargetPosition = true,
            float rotation = 0f)
        {
            var count = _targetables.Count;

            var relativeYRotation = faceTargetPosition
                ? RotationFromOriginToTarget(FindMiddlePointBetweenTargetables(), targetPoint)
                : rotation;
            var relativeRotation = Quaternion.Euler(0f, relativeYRotation, 0f);

            return _areaFormation.CalculatePositions(count, relativeRotation, targetPoint,
                _target.transform.position.y);
        }

        private float RotationFromOriginToTarget(Vector3 originPoint, Vector3 targetPoint)
        {
            var distToTarget = Vector3.Distance(originPoint, targetPoint);
            Vector3 differenceVector = targetPoint - originPoint;
            differenceVector.y = 0f;

            var angleBetween = Vector3.Angle(Vector3.forward * distToTarget, differenceVector);

            if (targetPoint.x > originPoint.x)
            {
                return angleBetween;
            }
            else
            {
                return 360f - angleBetween;
            }
        }

        private Vector3[] GenerateNoFormation(Vector3 targetPoint)
        {
            var noFormationPositions = new Vector3[_targetables.Count];
            for (var i = 0; i < _targetables.Count; i++)
            {
                noFormationPositions[i] = targetPoint;
            }

            return noFormationPositions;
        }

        private Vector3 FindMiddlePointBetweenTargetables()
        {
            var minPositionX = _targetables[0].Position.x;
            var maxPositionX = minPositionX;

            var minPositionZ = _targetables[0].Position.z;
            var maxPositionZ = minPositionZ;

            foreach (var unit in _targetables)
            {
                minPositionX = Mathf.Min(minPositionX, unit.Position.x);
                maxPositionX = Mathf.Max(maxPositionX, unit.Position.x);

                minPositionZ = Mathf.Min(minPositionZ, unit.Position.z);
                maxPositionZ = Mathf.Max(maxPositionZ, unit.Position.z);
            }

            var middlePoint = new Vector3(
                (minPositionX + maxPositionX) / 2, 0f, (minPositionZ + maxPositionZ) / 2);
            return middlePoint;
        }
    }
}
