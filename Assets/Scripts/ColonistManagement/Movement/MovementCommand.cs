using ColonistManagement.Targeting.Formations;
using Colonists;
using Colonists.Services.Selecting;
using General.Map;
using General.Selecting.Selected;
using ResourceManagement;
using Units;
using UnityEngine;
using Zenject;

namespace ColonistManagement.Movement
{
    [RequireComponent(typeof(MovementInput))]
    [RequireComponent(typeof(FormationMovement))]
    public class MovementCommand : MonoBehaviour
    {
        private SelectedColonists _selectedColonists;

        private FormationMovement _formationMovement;

        private MovementInput _movementInput;

        [Inject]
        public void Construct(SelectedColonists selectedColonists, MapInitialization mapInitialization)
        {
            _selectedColonists = selectedColonists;
        }

        private void Awake()
        {
            _movementInput = GetComponent<MovementInput>();
            _formationMovement = GetComponent<FormationMovement>();
        }

        private void OnEnable()
        {
            _movementInput.PositionSet += ShowFormation;
            _movementInput.RotationUpdate += RotateFormation;
            _movementInput.DestinationSet += FinishFormation;

            _movementInput.ColonistSet += OrderToColonist;
            _movementInput.UnitTarget += OrderToUnitTarget;
            _movementInput.ResourceSet += OrderToResource;

            _movementInput.Stop += Stop;
        }

        private void OnDisable()
        {
            _movementInput.PositionSet -= ShowFormation;
            _movementInput.RotationUpdate -= RotateFormation;
            _movementInput.DestinationSet -= FinishFormation;

            _movementInput.ColonistSet -= OrderToColonist;
            _movementInput.UnitTarget -= OrderToUnitTarget;
            _movementInput.ResourceSet -= OrderToResource;

            _movementInput.Stop -= Stop;
        }

        private void OrderToColonist(Colonist targetColonist)
        {
            foreach (var colonist in _selectedColonists.Colonists)
            {
                colonist.OrderTo(targetColonist);
            }
        }

        private void OrderToUnitTarget(Unit unit)
        {
            foreach (var colonist in _selectedColonists.Colonists)
            {
                colonist.OrderTo(unit);
            }
        }

        private void OrderToResource(Resource resource)
        {
            foreach (var colonist in _selectedColonists.Colonists)
            {
                colonist.OrderTo(resource);
            }
        }

        private void ShowFormation(Vector3 position, FormationColor formationColor)
        {
            _formationMovement.ShowFormation(position, _selectedColonists.Colonists, formationColor);
        }

        private void RotateFormation(float angle)
        {
            _formationMovement.RotateFormation(angle);
        }

        private void FinishFormation(bool additional, FormationColor formationColor)
        {
            _formationMovement.MoveToFormationPositions(additional, formationColor);
        }

        public void Stop()
        {
            foreach (var colonist in _selectedColonists.Colonists)
            {
                colonist.Stop();
            }
        }
    }
}
