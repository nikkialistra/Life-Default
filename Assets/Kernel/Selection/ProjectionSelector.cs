using System.Collections.Generic;
using Game.Units;
using Game.Units.Services;
using Kernel.Types;
using UnityEngine;

namespace Kernel.Selection
{
    public class ProjectionSelector
    {
        private readonly UnitRepository _unitRepository;
        private readonly Camera _camera;

        private IEnumerable<UnitFacade> _gameObjects;

        public ProjectionSelector(UnitRepository unitRepository, Camera camera)
        {
            _unitRepository = unitRepository;
            _camera = camera;
        }

        public IEnumerable<ISelectable> SelectInScreenSpace(Rect rect)
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
    }
}