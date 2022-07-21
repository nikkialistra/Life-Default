using ColonistManagement.Targeting.Formations;
using ColonistManagement.Targeting.Formations.Preview;
using Colonists;
using General.Selecting.Selected;
using ResourceManagement;
using Units;
using UnityEngine;
using Zenject;

namespace ColonistManagement.Movement
{
    [RequireComponent(typeof(MovementInput))]
    [RequireComponent(typeof(GroundTargeting))]
    [RequireComponent(typeof(FormationMovement))]
    public class MovementCommand : MonoBehaviour
    {
        private SelectedColonists _selectedColonists;

        private MovementInput _movementInput;
        private GroundTargeting _groundTargeting;

        private FormationMovement _formationMovement;

        [Inject]
        public void Construct(SelectedColonists selectedColonists)
        {
            _selectedColonists = selectedColonists;
        }

        private void Awake()
        {
            _movementInput = GetComponent<MovementInput>();
            _groundTargeting = GetComponent<GroundTargeting>();
            _formationMovement = GetComponent<FormationMovement>();
        }

        private void OnEnable()
        {
            _groundTargeting.PositionSet += ShowFormation;
            _groundTargeting.RotationUpdate += RotateFormation;
            _movementInput.DestinationSet += FinishFormation;

            _movementInput.ColonistSet += OrderToColonist;
            _movementInput.UnitTargetSet += OrderToUnitTarget;
            _movementInput.ResourceSet += OrderToResource;

            _movementInput.Stop += Stop;
        }

        private void OnDisable()
        {
            _groundTargeting.PositionSet -= ShowFormation;
            _groundTargeting.RotationUpdate -= RotateFormation;
            _movementInput.DestinationSet -= FinishFormation;

            _movementInput.ColonistSet -= OrderToColonist;
            _movementInput.UnitTargetSet -= OrderToUnitTarget;
            _movementInput.ResourceSet -= OrderToResource;

            _movementInput.Stop -= Stop;
        }

        public void Stop()
                {
                    foreach (var colonist in _selectedColonists.Colonists)
                        colonist.Stop();
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

        private void OrderToColonist(Colonist targetColonist)
        {
            foreach (var colonist in _selectedColonists.Colonists)
                colonist.OrderTo(targetColonist);
        }

        private void OrderToUnitTarget(Unit unit)
        {
            foreach (var colonist in _selectedColonists.Colonists)
                colonist.OrderTo(unit);
        }

        private void OrderToResource(Resource resource)
        {
            foreach (var colonist in _selectedColonists.Colonists)
                colonist.OrderTo(resource);
        }
    }
}
