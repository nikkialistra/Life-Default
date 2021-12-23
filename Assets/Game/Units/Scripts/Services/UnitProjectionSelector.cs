using System.Collections.Generic;
using Kernel.Types;
using UnityEngine;

namespace Game.Units.Services
{
    public class UnitProjectionSelector
    {
        private readonly UnitRepository _unitRepository;
        private readonly Camera _camera;

        private IEnumerable<UnitFacade> _gameObjects;

        public UnitProjectionSelector(UnitRepository unitRepository, Camera camera)
        {
            _unitRepository = unitRepository;
            _camera = camera;
        }

        public IEnumerable<ISelectable> SelectFromRect(Rect rect)
        {
            _gameObjects = _unitRepository.GetObjects();
            
            foreach (var gameObject in _gameObjects)
            {
                if (gameObject.GetComponent<ISelectable>() == null) continue;
                
                var screenPoint = _camera.WorldToScreenPoint(gameObject.transform.position);

                if (rect.Contains(screenPoint))
                {
                    yield return gameObject.GetComponent<ISelectable>();
                }
            }
        }
        public IEnumerable<ISelectable> SelectFromPoint(Vector2 point)
        {
            var ray = _camera.ScreenPointToRay(point);
            if (Physics.Raycast(ray, out var hit))
            {
                if (hit.transform.TryGetComponent(out ISelectable selectable))
                {
                    yield return selectable;
                }
            }
        }

    }
}