using Game.Units.Unit;
using UnityEngine;
using Zenject;

namespace Game.Units.Services
{
    public class UnitsGenerator : ITickable
    {
        private readonly UnitFacade.Factory _factory;

        private Vector3 _lastUnitPosition = Vector3.zero;

        public UnitsGenerator(UnitFacade.Factory factory)
        {
            _factory = factory;
        }

        public void Tick()
        {
            // if (Keyboard.current.spaceKey.wasPressedThisFrame)
            // {
            //     var unit = _factory.Create();
            //     unit.transform.position = _lastUnitPosition;
            //
            //     _lastUnitPosition += Vector3.forward * 2;
            // }
        }
    }
}