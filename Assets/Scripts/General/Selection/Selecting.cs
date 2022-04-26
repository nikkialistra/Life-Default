using System;
using System.Collections.Generic;
using Colonists;
using Colonists.Services;
using Enemies;
using Entities;
using General.Selection.Selected;
using pointcache.Frustum;
using UnityEngine;
using Zenject;

namespace General.Selection
{
    public class Selecting : MonoBehaviour
    {
        private ColonistRepository _colonistRepository;
        private Camera _camera;

        private readonly List<Colonist> _colonists = new();
        private readonly List<Enemy> _enemies = new();
        private readonly List<Entity> _entities = new();

        private SelectedColonists _selectedColonists;
        private SelectedEnemies _selectedEnemies;
        private SelectedEntities _selectedEntities;
        private FrustumSelector _frustumSelector;

        [Inject]
        public void Construct(ColonistRepository colonistRepository, Camera camera, FrustumSelector frustumSelector,
            SelectedColonists selectedColonists, SelectedEnemies selectedEnemies, SelectedEntities selectedEntities)
        {
            _colonistRepository = colonistRepository;
            _camera = camera;
            _frustumSelector = frustumSelector;

            _selectedColonists = selectedColonists;
            _selectedEnemies = selectedEnemies;
            _selectedEntities = selectedEntities;
        }

        private void OnEnable()
        {
            
        }

        private void OnDisable()
        {
            
        }

        public void SelectFromRect(Rect rect)
        {
            ClearAll();

            if (TrySelectColonistsFromRect(rect))
            {
                _selectedColonists.Set(_colonists);
            }

            TrySelectEntitiesFromRect(rect);
        }

        public void SelectFromPoint(Vector2 point)
        {
            ClearAll();

            if (TrySelectColonistsFromPoint(point))
            {
                _selectedColonists.Set(_colonists);
            }
        }

        private void Select(Collider collider)
        {
            Debug.Log(collider);
        }

        private void Deselect(Collider collider)
        {
            Debug.Log("-" + collider);
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
        
        private bool TrySelectEntitiesFromRect(Rect rect)
        {
            _frustumSelector.Select(rect);
            
            return true;
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
