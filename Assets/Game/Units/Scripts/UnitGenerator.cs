using UnityEngine;
using UnityEngine.InputSystem;
using Zenject;

namespace Game.Units.Scripts
{
    public class UnitGenerator : ITickable
    {
        private readonly Unit.Factory _factory;

        private Vector3 _lastUnitPosition;

        public UnitGenerator(Unit.Factory factory) => _factory = factory;

        public void Tick()
        {
            if (Keyboard.current.spaceKey.wasPressedThisFrame)
            {
                var unit = _factory.Create();
                unit.transform.position = _lastUnitPosition;

                _lastUnitPosition += Vector3.forward * 2;
            }
        }
    }
}