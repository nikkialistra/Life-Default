using System.Collections.Generic;
using Kernel.Saving;
using UnityEngine;
using Zenject;

namespace Game.Units.Services
{
    public class UnitsHandler : MonoBehaviour
    {
        private UnitFacade.Factory _factory;

        [Inject]
        public void Construct(UnitFacade.Factory factory)
        {
            _factory = factory;
        }
        
        public IEnumerable<UnitData> GetUnits()
        {
            foreach (var unitHandler in FindObjectsOfType<UnitHandler>())
            {
                var unitData = unitHandler.GetUnitData();
                yield return unitData;
            }
        }

        public void SetUnits(IEnumerable<UnitData> currentUnits)
        {
            DestroyUnits();
            
            foreach (var unitData in currentUnits)
            {
                var unitFacade = _factory.Create();
                unitFacade.UnitHandler.SetUnitData(unitData);
            }
        }

        private void DestroyUnits()
        {
            foreach (var unitHandler in FindObjectsOfType<UnitHandler>())
            {
                unitHandler.DestroySelf();
            }
        }
    }
}