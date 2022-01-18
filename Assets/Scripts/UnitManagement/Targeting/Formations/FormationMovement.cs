using System;
using System.Collections.Generic;
using Units.Unit;
using UnityEngine;
using Zenject;

namespace UnitManagement.Targeting.Formations
{
    [RequireComponent(typeof(AreaFormation))]
    public class FormationMovement : MonoBehaviour
    {
        [SerializeField] private FormationType _formationType;

        private List<UnitFacade> _units;
        private OrderMark _orderMark;

        private OrderMarkPool _orderMarkPool;

        private AreaFormation _areaFormation;

        [Inject]
        public void Construct(OrderMarkPool orderMarkPool)
        {
            _orderMarkPool = orderMarkPool;
        }

        private void Awake()
        {
            _areaFormation = GetComponent<AreaFormation>();
        }

        public void MoveTo(List<UnitFacade> units, OrderMark orderMark)
        {
            _units = units;
            _orderMark = orderMark;

            MoveUnitsToPositions(GenerateFormation(orderMark.transform.position));
        }

        private void MoveUnitsToPositions(Vector3[] formationPositions)
        {
            var assignedPositionsBitmask = new bool[formationPositions.Length];
            var assignedUnitsBitmask = new bool[formationPositions.Length];

            var middlePoint = FindMiddlePointBetweenUnits();

            for (var i = 0; i < formationPositions.Length; i++)
            {
                var formationIndex = _formationType == FormationType.Free
                    ? FarthestPointIndexFrom(formationPositions, assignedPositionsBitmask, middlePoint)
                    : i;

                var closestUnitIndex =
                    ClosestUnitIndexTo(formationPositions[formationIndex], assignedUnitsBitmask);

                assignedPositionsBitmask[formationIndex] = true;
                assignedUnitsBitmask[closestUnitIndex] = true;

                var unit = _units[closestUnitIndex];
                var formationPosition = formationPositions[formationIndex];
                if (TryOrder(unit, formationPosition))
                {
                    _orderMarkPool.Link(_orderMark, _units[closestUnitIndex]);
                }
            }
        }

        private bool TryOrder(UnitFacade unit, Vector3 formationPosition)
        {
            if (_orderMark.AtEntity)
            {
                return unit.TryOrderToEntityWithPosition(_orderMark.Entity, formationPosition);
            }
            else
            {
                return  unit.TryOrderToPosition(formationPosition);
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

        private int ClosestUnitIndexTo(Vector3 targetPoint, bool[] assignedUnitsBitmask)
        {
            var closestUnitDistance = 1000f;
            float distanceToPoint;
            var closestUnitIndex = 0;
            for (var i = 0; i < assignedUnitsBitmask.Length; i++)
            {
                if (assignedUnitsBitmask[i])
                {
                    continue;
                }

                distanceToPoint = Vector3.Distance(_units[i].transform.position, targetPoint);
                if (distanceToPoint < closestUnitDistance)
                {
                    closestUnitDistance = distanceToPoint;
                    closestUnitIndex = i;
                }
            }

            return closestUnitIndex;
        }

        private Vector3[] GenerateFormation(Vector3 targetPoint)
        {
            if (_units.Count == 0)
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
            var originPoint = FindMiddlePointBetweenUnits();

            var difference = targetPoint - originPoint;

            var freeFormationPositions = new Vector3[_units.Count];
            for (var i = 0; i < _units.Count; i++)
            {
                var targetPosition = _units[i].transform.position + difference;
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
            var count = _units.Count;

            var relativeYRotation = faceTargetPosition
                ? RotationFromOriginToTarget(FindMiddlePointBetweenUnits(), targetPoint)
                : rotation;
            var relativeRotation = Quaternion.Euler(0f, relativeYRotation, 0f);

            return _areaFormation.CalculatePositions(count, relativeRotation, targetPoint,
                _orderMark.transform.position.y);
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
            var noFormationPositions = new Vector3[_units.Count];
            for (var i = 0; i < _units.Count; i++)
            {
                noFormationPositions[i] = targetPoint;
            }

            return noFormationPositions;
        }

        private Vector3 FindMiddlePointBetweenUnits()
        {
            var minPositionX = _units[0].transform.position.x;
            var maxPositionX = minPositionX;

            var minPositionZ = _units[0].transform.position.z;
            var maxPositionZ = minPositionZ;

            foreach (var unit in _units)
            {
                minPositionX = Mathf.Min(minPositionX, unit.transform.position.x);
                maxPositionX = Mathf.Max(maxPositionX, unit.transform.position.x);

                minPositionZ = Mathf.Min(minPositionZ, unit.transform.position.z);
                maxPositionZ = Mathf.Max(maxPositionZ, unit.transform.position.z);
            }

            var middlePoint = new Vector3(
                (minPositionX + maxPositionX) / 2, 0f, (minPositionZ + maxPositionZ) / 2);
            return middlePoint;
        }
    }
}
