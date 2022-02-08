using System;
using System.Collections.Generic;
using System.Linq;
using Units.Unit;
using UnityEngine;
using UnityEngine.InputSystem;
using Zenject;

namespace UnitManagement.Targeting.Formations
{
    [RequireComponent(typeof(RegionFormation))]
    [RequireComponent(typeof(FormationPreviewDrawing))]
    public class FormationMovement : MonoBehaviour
    {
        [SerializeField] private FormationType _formationType;

        private List<UnitFacade> _units = new();
        private readonly List<Vector3> _unitPositions = new();
        private OrderMark _orderMark;

        private OrderMarkPool _orderMarkPool;

        private RegionFormation _regionFormation;

        private FormationPreviewDrawing _formationPreviewDrawing;

        private Vector3 _targetPoint;
        private Vector3[] _formationPositions;
        private float _relativeYRotation;

        private float _lastAngle;

        private bool _shown;

        private PlayerInput _playerInput;

        private InputAction _nextFormationAction;
        private InputAction _previousFormationAction;

        [Inject]
        public void Construct(OrderMarkPool orderMarkPool, PlayerInput playerInput)
        {
            _orderMarkPool = orderMarkPool;
            _playerInput = playerInput;
        }

        private void Awake()
        {
            _regionFormation = GetComponent<RegionFormation>();
            _formationPreviewDrawing = GetComponent<FormationPreviewDrawing>();

            _previousFormationAction = _playerInput.actions.FindAction("PreviousFormation");
            _nextFormationAction = _playerInput.actions.FindAction("NextFormation");
        }

        private void OnEnable()
        {
            _previousFormationAction.started += ChangeToPreviousFormation;
            _nextFormationAction.started += ChangeToNextFormation;
        }

        private void OnDisable()
        {
            _previousFormationAction.started -= ChangeToPreviousFormation;
            _nextFormationAction.started -= ChangeToNextFormation;
        }

        public void ShowFormation(List<UnitFacade> units, OrderMark orderMark)
        {
            _shown = true;
            _lastAngle = 0f;

            UnsubscribeFromUnits();
            _units = units;
            SubscribeToUnits();

            _unitPositions.Clear();
            foreach (var unit in _units)
            {
                _unitPositions.Add(unit.transform.position);
            }

            _orderMark = orderMark;

            Show();
        }

        private void SubscribeToUnits()
        {
            foreach (var unit in _units)
            {
                unit.UnitDie += RemoveFromFormation;
            }
        }

        private void UnsubscribeFromUnits()
        {
            foreach (var unit in _units)
            {
                unit.UnitDie -= RemoveFromFormation;
            }
        }

        private void RemoveFromFormation(UnitFacade unit)
        {
            _units.Remove(unit);

            if (_units.Count > 0)
            {
                Show();
            }
            else
            {
                _formationPreviewDrawing.Reset();
            }
        }

        public void RotateFormation(float angle)
        {
            if (_units.Count == 0 || _formationType == FormationType.None)
            {
                return;
            }

            _lastAngle = angle;

            var rotatedFormationPositions = GenerateFormationWithRotation(angle);
            _formationPreviewDrawing.UpdatePositions(rotatedFormationPositions, _regionFormation.CurrentYRotation);
        }

        public void MoveToFormationPositions()
        {
            _shown = false;

            if (_units.Count == 0)
            {
                return;
            }

            _formationPositions = GenerateFormationWithRotation(_lastAngle);

            UnsubscribeFromUnits();

            if (_formationPreviewDrawing.ShowDirectionArrow)
            {
                MoveUnitsToPositions(_formationPositions.Skip(1).ToArray());
            }
            else
            {
                MoveUnitsToPositions(_formationPositions);
            }

            _formationPreviewDrawing.Flash();
        }

        private void Show()
        {
            if (!_shown)
            {
                return;
            }

            _formationPositions = GenerateFormation(_orderMark.transform.position);

            if (_formationType == FormationType.None)
            {
                _formationPreviewDrawing.ShowNoFormationMark(_formationPositions[0]);
                return;
            }

            _formationPreviewDrawing.ShowDirectionArrow = FormationIsRegion;
            _formationPreviewDrawing.Show(_formationPositions, _regionFormation.CurrentYRotation);
        }

        private bool FormationIsRegion =>
            _formationType is FormationType.Area or FormationType.Line or FormationType.Sparse;

        private void ChangeToPreviousFormation(InputAction.CallbackContext context)
        {
            _formationType = _formationType switch
            {
                FormationType.Area => FormationType.None,
                FormationType.Line => FormationType.Area,
                FormationType.Sparse => FormationType.Line,
                FormationType.Free => FormationType.Sparse,
                FormationType.None => FormationType.Free,
                _ => throw new ArgumentOutOfRangeException()
            };

            Show();
        }

        private void ChangeToNextFormation(InputAction.CallbackContext context)
        {
            _formationType = _formationType switch
            {
                FormationType.Area => FormationType.Line,
                FormationType.Line => FormationType.Sparse,
                FormationType.Sparse => FormationType.Free,
                FormationType.Free => FormationType.None,
                FormationType.None => FormationType.Area,
                _ => throw new ArgumentOutOfRangeException()
            };

            Show();
        }

        private void MoveUnitsToPositions(Vector3[] formationPositions)
        {
            var assignedPositionsBitmask = new bool[formationPositions.Length];
            var assignedUnitsBitmask = new bool[formationPositions.Length];

            var middlePoint = FindMiddlePoint(_unitPositions);

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
                if (unit.TryOrderToPosition(formationPosition))
                {
                    _orderMarkPool.Link(_orderMark, _units[closestUnitIndex]);
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

        private int ClosestUnitIndexTo(Vector3 targetPoint, bool[] assignedUnitsBitmask)
        {
            var closestUnitDistance = 1000f;
            var closestUnitIndex = 0;
            for (var i = 0; i < assignedUnitsBitmask.Length; i++)
            {
                if (assignedUnitsBitmask[i])
                {
                    continue;
                }

                var distanceToPoint = Vector3.Distance(_units[i].transform.position, targetPoint);
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
                FormationType.Area => GenerateRegionFormation(targetPoint, RegionFormationType.Area),
                FormationType.Line => GenerateRegionFormation(targetPoint, RegionFormationType.Line),
                FormationType.Sparse => GenerateRegionFormation(targetPoint, RegionFormationType.Sparse),
                FormationType.Free => GenerateFreeFormation(targetPoint),
                FormationType.None => GenerateNoFormation(targetPoint),
                _ => throw new ArgumentOutOfRangeException()
            };
        }

        private Vector3[] GenerateFormationWithRotation(float rotation)
        {
            if (_units.Count == 0)
            {
                return Array.Empty<Vector3>();
            }

            if (FormationIsRegion)
            {
                return RotateRegionFormation(rotation);
            }
            else
            {
                return RotateFreeFormation(rotation);
            }
        }

        private Vector3[] GenerateRegionFormation(Vector3 targetPoint, RegionFormationType regionFormationType)
        {
            _targetPoint = targetPoint;

            var count = _units.Count;

            _relativeYRotation = RotationFromOriginToTarget(FindMiddlePoint(_unitPositions), targetPoint);

            var relativeRotation = Quaternion.Euler(0f, _relativeYRotation, 0f);

            return _regionFormation.CalculatePositions(count, relativeRotation, targetPoint,
                _orderMark.transform.position.y, regionFormationType);
        }

        private Vector3[] RotateRegionFormation(float rotation)
        {
            var relativeRotation = Quaternion.Euler(0f, _relativeYRotation + rotation, 0f);

            return _regionFormation.RotatePositions(relativeRotation);
        }

        private Vector3[] GenerateFreeFormation(Vector3 targetPoint)
        {
            var originPoint = FindMiddlePoint(_unitPositions);

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

        private Vector3[] RotateFreeFormation(float rotation)
        {
            var relativeRotation = Quaternion.Euler(0f, _relativeYRotation + rotation, 0f);
            var centerPoint = FindCenterPoint(_formationPositions);

            var rotatedPositions = new Vector3[_formationPositions.Length];
            for (var i = 0; i < _formationPositions.Length; i++)
            {
                var targetUnitPositionRotated =
                    centerPoint + relativeRotation * (_formationPositions[i] - centerPoint);

                rotatedPositions[i] =
                    new Vector3(targetUnitPositionRotated.x, _targetPoint.y, targetUnitPositionRotated.z);
            }

            return rotatedPositions;
        }

        private float RotationFromOriginToTarget(Vector3 originPoint, Vector3 targetPoint)
        {
            var distToTarget = Vector3.Distance(originPoint, targetPoint);
            var differenceVector = targetPoint - originPoint;
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

        private static Vector3 FindMiddlePoint(List<Vector3> positions)
        {
            if (positions.Count == 0)
            {
                throw new InvalidOperationException();
            }

            var minPositionX = positions[0].x;
            var maxPositionX = minPositionX;

            var minPositionZ = positions[0].z;
            var maxPositionZ = minPositionZ;

            foreach (var position in positions)
            {
                minPositionX = Mathf.Min(minPositionX, position.x);
                maxPositionX = Mathf.Max(maxPositionX, position.x);

                minPositionZ = Mathf.Min(minPositionZ, position.z);
                maxPositionZ = Mathf.Max(maxPositionZ, position.z);
            }

            var middlePoint = new Vector3(
                (minPositionX + maxPositionX) / 2, 0f, (minPositionZ + maxPositionZ) / 2);
            return middlePoint;
        }

        private static Vector3 FindCenterPoint(Vector3[] positions)
        {
            if (positions.Length == 0)
            {
                throw new InvalidOperationException();
            }

            var centerPoint = Vector3.zero;
            foreach (var position in positions)
            {
                centerPoint += position;
            }

            centerPoint /= positions.Length;

            return centerPoint;
        }
    }
}
