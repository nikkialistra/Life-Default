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

        private List<IOrderable> _orderables;
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

        public void MoveTo(List<IOrderable> orderables, OrderMark orderMark)
        {
            _orderables = orderables;
            _orderMark = orderMark;

            MoveOrderablesToPositions(GenerateFormation(orderMark.transform.position));
        }

        private void MoveOrderablesToPositions(Vector3[] formationPositions)
        {
            var assignedPositionsBitmask = new bool[formationPositions.Length];
            var assignedOrderablesBitmask = new bool[formationPositions.Length];

            var middlePoint = FindMiddlePointBetweenOrderables();

            for (var i = 0; i < formationPositions.Length; i++)
            {
                var formationIndex = _formationType == FormationType.Free
                    ? FarthestPointIndexFrom(formationPositions, assignedPositionsBitmask, middlePoint)
                    : i;

                var closestOrderableIndex =
                    ClosestOrderableIndexTo(formationPositions[formationIndex], assignedOrderablesBitmask);

                assignedPositionsBitmask[formationIndex] = true;
                assignedOrderablesBitmask[closestOrderableIndex] = true;

                var orderable = _orderables[closestOrderableIndex];
                var formationPosition = formationPositions[formationIndex];
                if (TryOrder(orderable, formationPosition))
                {
                    _orderMarkPool.Link(_orderMark, _orderables[closestOrderableIndex]);
                }
            }
        }

        private bool TryOrder(IOrderable orderable, Vector3 formationPosition)
        {
            if (_orderMark.HasTarget)
            {
                return orderable.TryOrderToTargetWithPosition(_orderMark.Target, formationPosition);
            }
            else
            {
                return  orderable.TryOrderToPosition(formationPosition);
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

        private int ClosestOrderableIndexTo(Vector3 targetPoint, bool[] assignedOrderablesBitmask)
        {
            var closestOrderableDistance = 1000f;
            float distanceToPoint;
            var closestOrderableIndex = 0;
            for (var i = 0; i < assignedOrderablesBitmask.Length; i++)
            {
                if (assignedOrderablesBitmask[i])
                {
                    continue;
                }

                distanceToPoint = Vector3.Distance(_orderables[i].Position, targetPoint);
                if (distanceToPoint < closestOrderableDistance)
                {
                    closestOrderableDistance = distanceToPoint;
                    closestOrderableIndex = i;
                }
            }

            return closestOrderableIndex;
        }

        private Vector3[] GenerateFormation(Vector3 targetPoint)
        {
            if (_orderables.Count == 0)
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
            var originPoint = FindMiddlePointBetweenOrderables();

            var difference = targetPoint - originPoint;

            var freeFormationPositions = new Vector3[_orderables.Count];
            for (var i = 0; i < _orderables.Count; i++)
            {
                var targetPosition = _orderables[i].Position + difference;
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
            var count = _orderables.Count;

            var relativeYRotation = faceTargetPosition
                ? RotationFromOriginToTarget(FindMiddlePointBetweenOrderables(), targetPoint)
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
            var noFormationPositions = new Vector3[_orderables.Count];
            for (var i = 0; i < _orderables.Count; i++)
            {
                noFormationPositions[i] = targetPoint;
            }

            return noFormationPositions;
        }

        private Vector3 FindMiddlePointBetweenOrderables()
        {
            var minPositionX = _orderables[0].Position.x;
            var maxPositionX = minPositionX;

            var minPositionZ = _orderables[0].Position.z;
            var maxPositionZ = minPositionZ;

            foreach (var unit in _orderables)
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
