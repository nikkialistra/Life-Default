using Entities.Entity;
using MapGeneration.Map;
using UnitManagement.Targeting.Formations;
using Units.Services.Selecting;
using UnityEngine;
using Zenject;

namespace UnitManagement.Targeting
{
    [RequireComponent(typeof(MovementInput))]
    [RequireComponent(typeof(FormationMovement))]
    public class MovementCommand : MonoBehaviour
    {
        private SelectedUnits _selectedUnits;
        private OrderMarkPool _orderMarkPool;

        private AstarPath _astarPath;
        private FormationMovement _formationMovement;

        private MovementInput _movementInput;

        [Inject]
        public void Construct(SelectedUnits selectedUnits, OrderMarkPool orderMarkPool, Map map, AstarPath astarPath)
        {
            _selectedUnits = selectedUnits;
            _orderMarkPool = orderMarkPool;
        }

        private void Awake()
        {
            _movementInput = GetComponent<MovementInput>();
            _formationMovement = GetComponent<FormationMovement>();
        }

        private void OnEnable()
        {
            _movementInput.EntitySet += MoveAllToEntity;
            _movementInput.PositionSet += MoveAllToPosition;
        }

        private void OnDisable()
        {
            _movementInput.EntitySet -= MoveAllToEntity;
            _movementInput.PositionSet -= MoveAllToPosition;
        }

        private void MoveAllToEntity(Entity entity)
        {
            if (EntityOrderedToSelf(entity))
            {
                return;
            }

            var destinationPoint = entity.GetDestinationPoint();
            var orderMark = _orderMarkPool.PlaceTo(destinationPoint, entity);
            MoveAllTo(orderMark);
        }

        private void MoveAllTo(OrderMark orderMark)
        {
            foreach (var unit in _selectedUnits.Units)
            {
                if (unit.TryOrderToEntity(orderMark.Entity))
                {
                    _orderMarkPool.Link(orderMark, unit);
                }
            }
        }

        private void MoveAllToPosition(Vector3 position)
        {
            var orderMark = _orderMarkPool.PlaceTo(position);
            MakeFormationTo(orderMark);
        }

        private void MakeFormationTo(OrderMark orderMark)
        {
            _formationMovement.MoveTo(_selectedUnits.Units, orderMark);
        }

        private bool EntityOrderedToSelf(Entity entity)
        {
            return _selectedUnits.Units.Count == 1 && _selectedUnits.Units[0].gameObject == entity.gameObject;
        }
    }
}
