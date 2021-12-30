using System.Collections.Generic;
using Units.Unit;
using Units.UnitTypes;
using UnityEngine;

namespace Units.Services.Selecting
{
    public class UnitsSelecting
    {
        private readonly UnitsRepository _unitsRepository;
        private readonly Camera _camera;

        private IEnumerable<UnitFacade> _units;

        public UnitsSelecting(UnitsRepository unitsRepository, Camera camera)
        {
            _unitsRepository = unitsRepository;
            _camera = camera;
        }

        public IEnumerable<UnitFacade> SelectFromRect(Rect rect)
        {
            _units = _unitsRepository.GetUnits();
            
            foreach (var unit in _units)
            {
                var screenPoint = _camera.WorldToScreenPoint(unit.transform.position);

                if (rect.Contains(screenPoint))
                {
                    yield return unit;
                }
            }
        }
        
        public IEnumerable<UnitFacade> SelectFromPoint(Vector2 point)
        {
            var ray = _camera.ScreenPointToRay(point);
            if (Physics.Raycast(ray, out var hit))
            {
                if (hit.transform.TryGetComponent(out UnitFacade unit) && unit.Alive)
                {
                    yield return unit;
                }
            }
        }

        public IEnumerable<UnitFacade> SelectByType(UnitType unitType)
        {
            _units = _unitsRepository.GetUnits();

            foreach (var unit in _units)
            {
                if (unit.UnitType == unitType)
                {
                    yield return unit;
                }
            }
        }
    }
}