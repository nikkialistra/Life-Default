using System;
using System.Collections.Generic;
using System.Linq;
using ColonistManagement.Targeting.Formations.Preview;
using ColonistManagement.Targeting.Formations.Region;
using Colonists;
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

        private List<Colonist> _colonists = new();
        private readonly List<Vector3> _colonistPositions = new();

        private RegionFormation _regionFormation;

        private FormationPreviewDrawing _formationPreviewDrawing;

        private Vector3 _targetPoint;
        private Vector3[] _formationPositions;
        private float _relativeYRotation;

        private float _lastAngle;

        private bool _shown;

        private FormationGeneration _formationGeneration;

        private PlayerInput _playerInput;

        private InputAction _nextFormationAction;
        private InputAction _previousFormationAction;

        [Inject]
        public void Construct(PlayerInput playerInput)
        {
            _playerInput = playerInput;
        }

        private void Awake()
        {
            _regionFormation = GetComponent<RegionFormation>();
            _formationPreviewDrawing = GetComponent<FormationPreviewDrawing>();

            _formationGeneration = new FormationGeneration(_regionFormation);

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

        public void ShowFormation(Vector3 position, List<Colonist> colonists, FormationColor formationColor)
        {
            _targetPoint = position;

            _shown = true;
            _lastAngle = 0f;

            UnsubscribeFromColonists();
            _colonists = colonists;
            SubscribeToColonists();

            _colonistPositions.Clear();
            foreach (var colonist in _colonists)
                _colonistPositions.Add(colonist.transform.position);

            Show(formationColor);
        }

        public void RotateFormation(float angle)
        {
            if (!_shown) throw new InvalidOperationException();

            if (_colonists.Count == 0 || _formationType == FormationType.None) return;

            _lastAngle = angle;

            var rotatedFormationPositions =
                _formationGeneration.GenerateWithRotation(angle, _colonists, _formationPositions, FormationIsRegion);
            _formationPreviewDrawing.UpdatePositions(rotatedFormationPositions, _regionFormation.CurrentYRotation);
        }

        public void MoveToFormationPositions(bool additional, FormationColor formationColor)
        {
            _shown = false;

            if (_colonists.Count == 0) return;

            _formationPreviewDrawing.ChangeColor(formationColor);

            _formationPositions =
                _formationGeneration.GenerateWithRotation(_lastAngle, _colonists, _formationPositions,
                    FormationIsRegion);

            UnsubscribeFromColonists();

            if (_formationPreviewDrawing.ShowDirectionArrow)
                MoveColonistsToPositions(_formationPositions.Skip(1).ToArray(), _regionFormation.CurrentYRotation,
                    additional);
            else
                MoveColonistsToPositions(_formationPositions, null, additional);

            _formationPreviewDrawing.Animate();
        }

        private void SubscribeToColonists()
        {
            foreach (var colonist in _colonists)
                colonist.ColonistDying += RemoveFromFormation;
        }

        private void UnsubscribeFromColonists()
        {
            foreach (var colonist in _colonists)
                colonist.ColonistDying -= RemoveFromFormation;
        }

        private void RemoveFromFormation(Colonist colonist)
        {
            _colonists.Remove(colonist);

            if (_colonists.Count > 0)
                Show();
            else
                _formationPreviewDrawing.Reset();
        }

        private void Show(FormationColor formationColor)
        {
            _formationPreviewDrawing.ChangeColor(formationColor);
            Show();
        }

        private void Show()
        {
            if (!_shown) return;

            _formationPositions =
                _formationGeneration.GenerateFormation(_targetPoint, _colonists, _colonistPositions, _formationType);

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
            var (sortedColonists, sortedFormationPositions) =
                FormationUtils.FindMappingBetweenColonistsAndPositions(formationPositions, _colonistPositions,
                _colonists, _formationType);

            for (int i = 0; i < sortedColonists.Length; i++)
                OrderUnit(sortedColonists[i], sortedFormationPositions[i], lastAngle, additional);
        }

        private void OrderUnit(Colonist colonist, Vector3 position, float? lastAngle, bool additional)
        {
            if (!additional)
                colonist.OrderToPosition(position, lastAngle);
            else
                colonist.TryAddPositionToOrder(position, lastAngle);
        }
    }
}
