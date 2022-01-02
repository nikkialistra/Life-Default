using System;
using Common;
using Units.Unit;
using Units.Unit.UnitTypes;
using UnityEngine;
using UnityEngine.InputSystem;
using Zenject;

namespace Testing
{
    public class UnitsGenerator : IInitializable, IDisposable
    {
        private readonly UnitFacade.Factory _factory;

        private readonly Camera _camera;

        private UnitType? _generationType;

        private readonly PlayerInput _playerInput;

        private InputAction _generateUnitAction;
        private InputAction _positionAction;
        private InputAction _changeGenerationToRandomAction;
        private InputAction _changeGenerationToTravelersAction;
        private InputAction _changeGenerationToLumberjacksAction;
        private InputAction _changeGenerationToMasonsAction;
        private InputAction _changeGenerationToMeleesAction;
        private InputAction _changeGenerationToArchersAction;

        public UnitsGenerator(UnitFacade.Factory factory, Camera camera, PlayerInput playerInput)
        {
            _factory = factory;
            _camera = camera;
            _playerInput = playerInput;
        }

        private void GenerateUnit(InputAction.CallbackContext context)
        {
            var mousePosition = _positionAction.ReadValue<Vector2>();
            var ray = _camera.ScreenPointToRay(new Vector3(mousePosition.x, mousePosition.y, _camera.nearClipPlane));
            if (Physics.Raycast(ray, out var hit))
            {
                var unitType = GetUnitType();
                _factory.Create(unitType, hit.point);
            }
        }

        private UnitType GetUnitType()
        {
            if (_generationType != null)
            {
                return (UnitType)_generationType;
            }
            else
            {
                return EnumUtils.RandomEnumValue<UnitType>();
            }
        }

        public void Initialize()
        {
            _generateUnitAction = _playerInput.actions.FindAction("TestingGenerateUnit");
            _positionAction = _playerInput.actions.FindAction("Position");

            _changeGenerationToRandomAction = _playerInput.actions.FindAction("TestingChangeGenerationToRandom");
            _changeGenerationToTravelersAction = _playerInput.actions.FindAction("TestingChangeGenerationToTravelers");
            _changeGenerationToLumberjacksAction =
                _playerInput.actions.FindAction("TestingChangeGenerationToLumberjacks");
            _changeGenerationToMasonsAction = _playerInput.actions.FindAction("TestingChangeGenerationToMasons");
            _changeGenerationToMeleesAction = _playerInput.actions.FindAction("TestingChangeGenerationToMelees");
            _changeGenerationToArchersAction = _playerInput.actions.FindAction("TestingChangeGenerationToArchers");

            _generateUnitAction.started += GenerateUnit;

            _changeGenerationToRandomAction.started += ChangeGenerationToRandom;
            _changeGenerationToTravelersAction.started += ChangeGenerationToTravelers;
            _changeGenerationToLumberjacksAction.started += ChangeGenerationToLumberjacks;
            _changeGenerationToMasonsAction.started += ChangeGenerationToMasons;
            _changeGenerationToMeleesAction.started += ChangeGenerationToMelees;
            _changeGenerationToArchersAction.started += ChangeGenerationToArchers;
        }

        public void Dispose()
        {
            _generateUnitAction.started -= GenerateUnit;

            _changeGenerationToRandomAction.started -= ChangeGenerationToRandom;
            _changeGenerationToTravelersAction.started -= ChangeGenerationToTravelers;
            _changeGenerationToLumberjacksAction.started -= ChangeGenerationToLumberjacks;
            _changeGenerationToMasonsAction.started -= ChangeGenerationToMasons;
            _changeGenerationToMeleesAction.started -= ChangeGenerationToMelees;
            _changeGenerationToArchersAction.started -= ChangeGenerationToArchers;
        }

        private void ChangeGenerationToRandom(InputAction.CallbackContext context)
        {
            _generationType = null;
        }

        private void ChangeGenerationToTravelers(InputAction.CallbackContext context)
        {
            _generationType = UnitType.Traveler;
        }

        private void ChangeGenerationToLumberjacks(InputAction.CallbackContext context)
        {
            _generationType = UnitType.Lumberjack;
        }

        private void ChangeGenerationToMasons(InputAction.CallbackContext context)
        {
            _generationType = UnitType.Mason;
        }

        private void ChangeGenerationToMelees(InputAction.CallbackContext context)
        {
            _generationType = UnitType.Melee;
        }

        private void ChangeGenerationToArchers(InputAction.CallbackContext context)
        {
            _generationType = UnitType.Archer;
        }
    }
}