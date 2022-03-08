using System;
using System.Collections.Generic;
using System.Linq;
using ColonistManagement.OrderMarks;
using Colonists.Colonist;
using UnityEngine;
using UnityEngine.InputSystem;
using Zenject;

namespace ColonistManagement.Targeting.Formations
{
    [RequireComponent(typeof(RegionFormation))]
    [RequireComponent(typeof(FormationPreviewDrawing))]
    public class FormationMovement : MonoBehaviour
    {
        [SerializeField] private FormationType _formationType;

        private List<ColonistFacade> _colonists = new();
        private readonly List<Vector3> _colonistPositions = new();
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

            _previousFormationAction = _playerInput.actions.FindAction("Previous Formation");
            _nextFormationAction = _playerInput.actions.FindAction("Next Formation");
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

        public void ShowFormation(List<ColonistFacade> colonists, OrderMark orderMark, FormationColor formationColor)
        {
            _shown = true;
            _lastAngle = 0f;

            UnsubscribeFromColonists();
            _colonists = colonists;
            SubscribeToColonists();

            _colonistPositions.Clear();
            foreach (var colonist in _colonists)
            {
                _colonistPositions.Add(colonist.transform.position);
            }

            _orderMark = orderMark;

            Show(formationColor);
        }

        public void RotateFormation(float angle)
        {
            if (!_shown)
            {
                throw new InvalidOperationException();
            }

            if (_colonists.Count == 0 || _formationType == FormationType.None)
            {
                return;
            }

            _lastAngle = angle;

            var rotatedFormationPositions = GenerateFormationWithRotation(angle);
            _formationPreviewDrawing.UpdatePositions(rotatedFormationPositions, _regionFormation.CurrentYRotation);
        }

        public void MoveToFormationPositions(bool additional, FormationColor formationColor)
        {
            _shown = false;

            if (_colonists.Count == 0)
            {
                return;
            }

            _formationPreviewDrawing.ChangeColor(formationColor);

            _formationPositions = GenerateFormationWithRotation(_lastAngle);

            UnsubscribeFromColonists();

            if (_formationPreviewDrawing.ShowDirectionArrow)
            {
                MoveColonistsToPositions(_formationPositions.Skip(1).ToArray(), _regionFormation.CurrentYRotation,
                    additional);
            }
            else
            {
                MoveColonistsToPositions(_formationPositions, null, additional);
            }

            _formationPreviewDrawing.Animate();
        }

        private void SubscribeToColonists()
        {
            foreach (var colonist in _colonists)
            {
                colonist.ColonistDie += RemoveFromFormation;
            }
        }

        private void UnsubscribeFromColonists()
        {
            foreach (var colonist in _colonists)
            {
                colonist.ColonistDie -= RemoveFromFormation;
            }
        }

        private void RemoveFromFormation(ColonistFacade colonist)
        {
            _colonists.Remove(colonist);

            if (_colonists.Count > 0)
            {
                Show();
            }
            else
            {
                _formationPreviewDrawing.Reset();
            }
        }

        private void Show(FormationColor formationColor)
        {
            _formationPreviewDrawing.ChangeColor(formationColor);
            Show();
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
            _formationType is FormationType.Area or FormationType.Line or FormationType.Scattered;

        private void ChangeToPreviousFormation(InputAction.CallbackContext context)
        {
            _formationType = _formationType switch
            {
                FormationType.Area => FormationType.None,
                FormationType.Line => FormationType.Area,
                FormationType.Scattered => FormationType.Line,
                FormationType.Free => FormationType.Scattered,
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
                FormationType.Line => FormationType.Scattered,
                FormationType.Scattered => FormationType.Free,
                FormationType.Free => FormationType.None,
                FormationType.None => FormationType.Area,
                _ => throw new ArgumentOutOfRangeException()
            };

            Show();
        }

        private void MoveColonistsToPositions(Vector3[] formationPositions, float? lastAngle, bool additional)
        {
            var orderedColonists = new ColonistFacade[formationPositions.Length];
            var orderedFormationPositions = new Vector3[formationPositions.Length];

            FindMappingBetweenColonistsAndPositions(formationPositions, orderedFormationPositions, orderedColonists);

            for (var i = 0; i < orderedColonists.Length; i++)
            {
                OrderUnit(orderedColonists[i], formationPositions[i], lastAngle, additional);
            }
        }

        private void OrderUnit(ColonistFacade colonist, Vector3 position, float? lastAngle, bool additional)
        {
            if (!additional)
            {
                if (colonist.TryOrderToPosition(position, lastAngle))
                {
                    _orderMarkPool.Link(_orderMark, colonist);
                }
            }
            else
            {
                if (colonist.TryAddPositionToOrder(position, lastAngle))
                {
                    _orderMarkPool.Link(_orderMark, colonist);
                }
            }
        }

        private void FindMappingBetweenColonistsAndPositions(Vector3[] formationPositions,
            Vector3[] orderedFormationPositions,
            ColonistFacade[] orderedColonists)
        {
            var assignedPositionsBitmask = new bool[formationPositions.Length];
            var assignedColonistsBitmask = new bool[formationPositions.Length];

            var middlePoint = FormationUtils.FindMiddlePoint(_colonistPositions);

            for (var i = 0; i < formationPositions.Length; i++)
            {
                var formationIndex = _formationType == FormationType.Free
                    ? FormationUtils.FarthestPointIndexFrom(formationPositions, assignedPositionsBitmask, middlePoint)
                    : i;

                var closestUnitIndex =
                    FormationUtils.ClosestUnitIndexTo(_colonists, formationPositions[formationIndex], assignedColonistsBitmask);

                assignedPositionsBitmask[formationIndex] = true;
                assignedColonistsBitmask[closestUnitIndex] = true;

                orderedFormationPositions[i] = formationPositions[formationIndex];
                orderedColonists[i] = _colonists[closestUnitIndex];
            }
        }

        private Vector3[] GenerateFormation(Vector3 targetPoint)
        {
            if (_colonists.Count == 0)
            {
                return Array.Empty<Vector3>();
            }

            return _formationType switch
            {
                FormationType.Area => GenerateRegionFormation(targetPoint, RegionFormationType.Area),
                FormationType.Line => GenerateRegionFormation(targetPoint, RegionFormationType.Line),
                FormationType.Scattered => GenerateRegionFormation(targetPoint, RegionFormationType.Scattered),
                FormationType.Free => GenerateFreeFormation(targetPoint),
                FormationType.None => GenerateNoFormation(targetPoint),
                _ => throw new ArgumentOutOfRangeException()
            };
        }

        private Vector3[] GenerateFormationWithRotation(float rotation)
        {
            if (_colonists.Count == 0)
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

            var count = _colonists.Count;

            _relativeYRotation =
                FormationUtils.RotationFromOriginToTarget(FormationUtils.FindMiddlePoint(_colonistPositions), targetPoint);

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
            var originPoint = FormationUtils.FindMiddlePoint(_colonistPositions);

            var difference = targetPoint - originPoint;

            var freeFormationPositions = new Vector3[_colonists.Count];
            for (var i = 0; i < _colonists.Count; i++)
            {
                var targetPosition = _colonists[i].transform.position + difference;
                freeFormationPositions[i] = new Vector3(
                    targetPosition.x, targetPoint.y, targetPosition.z
                );
            }

            return freeFormationPositions;
        }

        private Vector3[] RotateFreeFormation(float rotation)
        {
            var relativeRotation = Quaternion.Euler(0f, _relativeYRotation + rotation, 0f);
            var centerPoint = FormationUtils.FindCenterPoint(_formationPositions);

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

        private Vector3[] GenerateNoFormation(Vector3 targetPoint)
        {
            var noFormationPositions = new Vector3[_colonists.Count];
            for (var i = 0; i < _colonists.Count; i++)
            {
                noFormationPositions[i] = targetPoint;
            }

            return noFormationPositions;
        }
    }
}
