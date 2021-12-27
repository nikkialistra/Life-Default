using Game.Units.Unit;
using UnityEngine;
using Zenject;

namespace Game.Units.Services
{
    public class UnitsGenerator : IInitializable
    {
        private readonly UnitFacade.Factory _factory;
        
        public UnitsGenerator(UnitFacade.Factory factory)
        {
            _factory = factory;
        }

        public void Initialize()
        {
            _factory.Create(Vector3.zero);
        }
    }
}