using UnityEngine;
using UnityEngine.InputSystem;
using Zenject;

namespace Game.Units
{
    public class UnitGenerator : ITickable
    {
        private readonly UnitFacade.Factory _factory;

        private Vector3 _lastUnitPosition = Vector3.zero;

        public UnitGenerator(UnitFacade.Factory factory) => _factory = factory;

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