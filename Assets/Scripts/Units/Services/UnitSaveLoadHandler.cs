using System.Collections.Generic;
using Units.Unit;
using Units.Unit.UnitTypes;
using UnityEngine;
using Zenject;

namespace Units.Services
{
    public class UnitSaveLoadHandler : MonoBehaviour
    {
        private UnitFacade.Factory _factory;

        [Inject]
        public void Construct(UnitFacade.Factory factory)
        {
            _factory = factory;
        }

        public static List<UnitData> GetUnits()
        {
            var units = new List<UnitData>();
            foreach (var unitHandler in FindObjectsOfType<Unit.UnitSaveLoadHandler>())
            {
                var unitData = unitHandler.GetUnitData();
                units.Add(unitData);
            }

            return units;
        }

        public void SetUnits(IEnumerable<UnitData> currentUnits)
        {
            DestroyUnits();

            foreach (var unitData in currentUnits)
            {
                var unitFacade = _factory.Create(UnitType.Scout, Vector3.zero);
                unitFacade.UnitSaveLoadHandler.SetUnitData(unitData);
            }
        }

        private void DestroyUnits()
        {
            foreach (var unitHandler in FindObjectsOfType<Unit.UnitSaveLoadHandler>())
            {
                unitHandler.DestroySelf();
            }
        }
    }
}
