using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace Game.Units.Services
{
    public class SelectedUnits
    {
        private readonly UnitRepository _unitRepository;

        public SelectedUnits(UnitRepository unitRepository)
        {
            _unitRepository = unitRepository;
        }
        
        public IEnumerable<UnitFacade> Units { get; private set; } = Array.Empty<UnitFacade>();

        public void Clear()
        {
            Units = Array.Empty<UnitFacade>();
            
            var allUnits = _unitRepository.GetObjects();
            foreach (var unit in allUnits)
            {
                unit.OnDeselect();
            }
        }

        public void Set(IEnumerable<UnitFacade> units)
        {
            Units = units as UnitFacade[] ?? units.ToArray();
            
            var allUnits = _unitRepository.GetObjects();
            
            foreach (var deselected in allUnits.Except(Units))
            {
                deselected.OnDeselect();
            }

            foreach (var selected in Units)
            {
                selected.OnSelect();
            }
        }
    }
}