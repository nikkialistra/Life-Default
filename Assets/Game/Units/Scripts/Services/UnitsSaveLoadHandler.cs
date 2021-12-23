﻿using System.Collections.Generic;
using UnityEngine;
using Zenject;

namespace Game.Units.Services
{
    public class UnitsSaveLoadHandler : MonoBehaviour
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
            foreach (var unitHandler in FindObjectsOfType<UnitSaveLoadHandler>())
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
                var unitFacade = _factory.Create();
                unitFacade.UnitSaveLoadHandler.SetUnitData(unitData);
            }
        }

        private void DestroyUnits()
        {
            foreach (var unitHandler in FindObjectsOfType<UnitSaveLoadHandler>())
            {
                unitHandler.DestroySelf();
            }
        }
    }
}