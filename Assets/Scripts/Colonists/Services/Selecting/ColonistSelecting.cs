using System.Collections.Generic;
using Colonists.Colonist;
using UnityEngine;

namespace Colonists.Services.Selecting
{
    public class ColonistSelecting
    {
        private readonly ColonistRepository _colonistRepository;
        private readonly Camera _camera;

        private IEnumerable<ColonistFacade> _colonists;

        public ColonistSelecting(ColonistRepository colonistRepository, Camera camera)
        {
            _colonistRepository = colonistRepository;
            _camera = camera;
        }

        public IEnumerable<ColonistFacade> SelectFromRect(Rect rect)
        {
            _colonists = _colonistRepository.GetColonists();

            foreach (var colonist in _colonists)
            {
                var screenPoint = _camera.WorldToScreenPoint(colonist.Center);

                if (rect.Contains(screenPoint))
                {
                    yield return colonist;
                }
            }
        }

        public IEnumerable<ColonistFacade> SelectFromPoint(Vector2 point)
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

        private static IEnumerable<ColonistFacade> GetUnitsFromHit(RaycastHit hit)
        {
            if (hit.transform.TryGetComponent(out ColonistFacade clickedUnit) && clickedUnit.Alive)
            {
                yield return clickedUnit;
            }
        }
    }
}
