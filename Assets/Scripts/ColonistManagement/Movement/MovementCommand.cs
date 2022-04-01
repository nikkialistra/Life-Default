using ColonistManagement.OrderMarks;
using ColonistManagement.Targeting.Formations;
using Colonists.Services.Selecting;
using Entities;
using General;
using UnityEngine;
using Zenject;

namespace ColonistManagement.Movement
{
    [RequireComponent(typeof(MovementInput))]
    [RequireComponent(typeof(FormationMovement))]
    public class MovementCommand : MonoBehaviour
    {
        private SelectedColonists _selectedColonists;
        private OrderMarkPool _orderMarkPool;
        
        private FormationMovement _formationMovement;

        private MovementInput _movementInput;

        [Inject]
        public void Construct(SelectedColonists selectedColonists, OrderMarkPool orderMarkPool, Map map)
        {
            _selectedColonists = selectedColonists;
            _orderMarkPool = orderMarkPool;
        }

        private void Awake()
        {
            _movementInput = GetComponent<MovementInput>();
            _formationMovement = GetComponent<FormationMovement>();
        }

        private void OnEnable()
        {
            _movementInput.EntitySet += MoveToEntity;

            _movementInput.PositionSet += ShowFormation;
            _movementInput.RotationUpdate += RotateFormation;
            _movementInput.DestinationSet += FinishFormation;

            _movementInput.Stop += Stop;
        }

        private void OnDisable()
        {
            _movementInput.EntitySet -= MoveToEntity;

            _movementInput.PositionSet -= ShowFormation;
            _movementInput.RotationUpdate -= RotateFormation;
            _movementInput.DestinationSet -= FinishFormation;

            _movementInput.Stop -= Stop;
        }

        private void MoveToEntity(Entity entity)
        {
            if (EntityOrderedToSelf(entity))
            {
                return;
            }

            var destinationPoint = entity.GetDestinationPoint();
            var orderMark = _orderMarkPool.PlaceTo(destinationPoint, entity);
            MoveTo(orderMark);
        }

        private void ShowFormation(Vector3 position, FormationColor formationColor)
        {
            var orderMark = _orderMarkPool.PlaceTo(position);
            _formationMovement.ShowFormation(_selectedColonists.Colonists, orderMark, formationColor);
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

        private void MoveTo(OrderMark orderMark)
        {
            foreach (var colonist in _selectedColonists.Colonists)
            {
                if (colonist.TryOrderToEntity(orderMark.Entity))
                {
                    _orderMarkPool.Link(orderMark, colonist);
                }
            }
        }

        private bool EntityOrderedToSelf(Entity entity)
        {
            return _selectedColonists.Colonists.Count == 1 && _selectedColonists.Colonists[0].gameObject == entity.gameObject;
        }
    }
}
