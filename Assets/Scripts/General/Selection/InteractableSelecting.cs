using System.Collections.Generic;
using Colonists;
using Colonists.Services;
using Enemies;
using Entities;
using General.Selection.Selected;
using UnityEngine;

namespace General.Selection
{
    public class InteractableSelecting
    {
        private readonly ColonistRepository _colonistRepository;
        private readonly Camera _camera;

        private readonly List<Colonist> _colonists = new();
        private readonly List<Enemy> _enemies = new();
        private readonly List<Entity> _entities = new();

        private readonly SelectedColonists _selectedColonists;
        private readonly SelectedEnemies _selectedEnemies;
        private readonly SelectedEntities _selectedEntities;

        public InteractableSelecting(ColonistRepository colonistRepository, Camera camera,
            SelectedColonists selectedColonists, SelectedEnemies selectedEnemies, SelectedEntities selectedEntities)
        {
            _colonistRepository = colonistRepository;
            _camera = camera;

            _selectedColonists = selectedColonists;
            _selectedEnemies = selectedEnemies;
            _selectedEntities = selectedEntities;
        }

        public void SelectFromRect(Rect rect)
        {
            ClearAll();

            if (TrySelectColonistsFromRect(rect))
            {
                _selectedColonists.Set(_colonists);
            }
        }

        public void SelectFromPoint(Vector2 point)
        {
            ClearAll();

            if (TrySelectColonistsFromPoint(point))
            {
                _selectedColonists.Set(_colonists);
            }
        }

        private bool TrySelectColonistsFromRect(Rect rect)
        {
            var allColonists = _colonistRepository.GetColonists();

            foreach (var colonist in allColonists)
            {
                var screenPoint = _camera.WorldToScreenPoint(colonist.Center);

                if (rect.Contains(screenPoint))
                {
                    _colonists.Add(colonist);
                }
            }

            return _colonists.Count > 0;
        }

        private bool TrySelectColonistsFromPoint(Vector2 point)
        {
            var ray = _camera.ScreenPointToRay(point);

            if (Physics.Raycast(ray, out var hit))
            {
                if (hit.transform.TryGetComponent(out Colonist colonist) && colonist.Alive)
                {
                    _colonists.Add(colonist);
                    return true;
                }
            }

            return false;
        }

        private void ClearAll()
        {
            _selectedColonists.Clear();
            _selectedEnemies.Clear();
            _selectedEntities.Clear();

            _colonists.Clear();
            _enemies.Clear();
            _entities.Clear();
        }
    }
}
