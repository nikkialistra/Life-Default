using System.Collections.Generic;
using Kernel.Types;
using UnityEngine;

namespace Game.Units.Services
{
    public class UnitSelector
    {
        private readonly UnitRepository _unitRepository;
        private readonly Camera _camera;

        private IEnumerable<UnitFacade> _units;

        public UnitSelector(UnitRepository unitRepository, Camera camera)
        {
            _unitRepository = unitRepository;
            _camera = camera;
        }

        public IEnumerable<UnitFacade> SelectFromRect(Rect rect)
        {
            _units = _unitRepository.GetObjects();
            
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
                if (hit.transform.TryGetComponent(out UnitFacade unit))
                {
                    yield return unit;
                }
            }
        }

        public IEnumerable<UnitFacade> SelectByType(UnitType unitType)
        {
            _units = _unitRepository.GetObjects();

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