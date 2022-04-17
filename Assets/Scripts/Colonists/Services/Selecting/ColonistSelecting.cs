using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace Colonists.Services.Selecting
{
    public class ColonistSelecting
    {
        private readonly ColonistRepository _colonistRepository;
        private readonly Camera _camera;

        private IEnumerable<Colonist> _colonists;

        public ColonistSelecting(ColonistRepository colonistRepository, Camera camera)
        {
            _colonistRepository = colonistRepository;
            _camera = camera;
        }

        public IEnumerable<Colonist> SelectFromRect(Rect rect)
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

        public IEnumerable<Colonist> SelectFromPoint(Vector2 point)
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

        private static IEnumerable<Colonist> GetColonistsFromHit(RaycastHit hit)
        {
            if (hit.transform.TryGetComponent(out Colonist clickedColonist) && clickedColonist.Alive)
            {
                yield return clickedColonist;
            }
        }
    }
}
