﻿using Entities;
using MapGeneration;
using UnitManagement.OrderMarks;
using UnitManagement.Targeting.Formations;
using Units.Services.Selecting;
using UnityEngine;
using Zenject;

namespace UnitManagement.Movement
{
    [RequireComponent(typeof(MovementInput))]
    [RequireComponent(typeof(FormationMovement))]
    public class MovementCommand : MonoBehaviour
    {
        private SelectedUnits _selectedUnits;
        private OrderMarkPool _orderMarkPool;
        
        private FormationMovement _formationMovement;

        private MovementInput _movementInput;

        [Inject]
        public void Construct(SelectedUnits selectedUnits, OrderMarkPool orderMarkPool, Map map)
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
            _formationMovement.ShowFormation(_selectedUnits.Units, orderMark, formationColor);
        }

        private void RotateFormation(float angle)
        {
            _formationMovement.RotateFormation(angle);
        }

        private void FinishFormation(bool additional, FormationColor formationColor)
        {
            _formationMovement.MoveToFormationPositions(additional, formationColor);
        }

        private void Stop()
        {
            foreach (var unit in _selectedUnits.Units)
            {
                unit.Stop();
            }
        }

        private void MoveTo(OrderMark orderMark)
        {
            foreach (var unit in _selectedUnits.Units)
            {
                if (unit.TryOrderToEntity(orderMark.Entity))
                {
                    _orderMarkPool.Link(orderMark, unit);
                }
            }
        }

        private bool EntityOrderedToSelf(Entity entity)
        {
            return _selectedUnits.Units.Count == 1 && _selectedUnits.Units[0].gameObject == entity.gameObject;
        }
    }
}
