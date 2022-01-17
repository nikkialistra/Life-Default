using Common;
using Units.Unit;
using Units.Unit.UnitType;
using UnityEngine;
using UnityEngine.InputSystem;
using Zenject;

namespace Testing
{
    public class UnitsGenerator : ITickable
    {
        private UnitType? _generationType = UnitType.Lumberjack;
        private readonly LayerMask _terrainMask = LayerMask.GetMask("Terrain");

        private readonly UnitFacade.Factory _factory;

        private readonly Camera _camera;

        public UnitsGenerator(UnitFacade.Factory factory, Camera camera)
        {
            _factory = factory;
            _camera = camera;
        }

        public void Tick()
        {
            if (!Keyboard.current.altKey.isPressed)
            {
                return;
            }

            CheckForGenerateCommand();

            CheckForSwitchingGenerationTypeCommand();
        }

        private void CheckForGenerateCommand()
        {
            if (Mouse.current.leftButton.wasPressedThisFrame)
            {
                GenerateUnit(Mouse.current.position.ReadValue());
            }
        }

        private void CheckForSwitchingGenerationTypeCommand()
        {
            if (Keyboard.current.digit1Key.isPressed)
            {
                _generationType = UnitType.Scout;
            }

            if (Keyboard.current.digit2Key.isPressed)
            {
                _generationType = UnitType.Lumberjack;
            }

            if (Keyboard.current.digit3Key.isPressed)
            {
                _generationType = UnitType.Mason;
            }

            if (Keyboard.current.digit4Key.isPressed)
            {
                _generationType = UnitType.Melee;
            }

            if (Keyboard.current.digit5Key.isPressed)
            {
                _generationType = UnitType.Archer;
            }

            if (Keyboard.current.digit6Key.isPressed)
            {
                _generationType = null;
            }
        }

        private void GenerateUnit(Vector2 position)
        {
            var ray = _camera.ScreenPointToRay(new Vector3(position.x, position.y, _camera.nearClipPlane));
            if (Physics.Raycast(ray, out var hit, float.PositiveInfinity, _terrainMask))
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
    }
}
