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
                foreach (var colonist in GetColonistsFromHit(hit))
                {
                    yield return colonist;
                }
            }
        }

        private static IEnumerable<ColonistFacade> GetColonistsFromHit(RaycastHit hit)
        {
            if (hit.transform.TryGetComponent(out ColonistFacade clickedUnit) && clickedUnit.Alive)
            {
                yield return clickedUnit;
            }
        }
    }
}
