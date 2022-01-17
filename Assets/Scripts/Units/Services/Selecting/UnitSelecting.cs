using System.Collections.Generic;
using UnitManagement.Targeting;
using Units.Unit;
using Units.Unit.UnitType;
using UnityEngine;

namespace Units.Services.Selecting
{
    public class UnitSelecting
    {
        private readonly UnitRepository _unitRepository;
        private readonly Camera _camera;

        private IEnumerable<UnitFacade> _units;

        public UnitSelecting(UnitRepository unitRepository, Camera camera)
        {
            _unitRepository = unitRepository;
            _camera = camera;
        }

        public IEnumerable<UnitFacade> SelectFromRect(Rect rect)
        {
            _units = _unitRepository.GetUnits();

            foreach (var unit in _units)
            {
                var screenPoint = _camera.WorldToScreenPoint(unit.Center);

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
                foreach (var unitFacade in GetUnitsFromHit(hit))
                {
                    yield return unitFacade;
                }
            }
        }

        private static IEnumerable<UnitFacade> GetUnitsFromHit(RaycastHit hit)
        {
            if (hit.transform.TryGetComponent(out UnitFacade clickedUnit) && clickedUnit.Alive)
            {
                yield return clickedUnit;
            }
            else if (hit.transform.TryGetComponent(out TargetMark target))
            {
                foreach (var targetable in target.Targetables)
                {
                    if (targetable.GameObject.TryGetComponent(out UnitFacade targetableUnit) && targetableUnit.Alive)
                    {
                        yield return targetableUnit;
                    }
                }
            }
        }

        public IEnumerable<UnitFacade> SelectByType(UnitType unitType)
        {
            _units = _unitRepository.GetUnits();

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
