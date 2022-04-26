using System.Collections.Generic;
using Colonists;
using Enemies;
using Entities;
using General.Selection.Selected;
using Units;
using Units.Enums;
using UnityEngine;
using Zenject;

namespace General.Selection
{
    public class Selecting : MonoBehaviour
    {
        private Camera _camera;

        private readonly List<Colonist> _colonists = new();
        private readonly List<Enemy> _enemies = new();
        private readonly List<Entity> _entities = new();

        private SelectedColonists _selectedColonists;
        private SelectedEnemies _selectedEnemies;
        private SelectedEntities _selectedEntities;
        private FrustumSelector _frustumSelector;

        [Inject]
        public void Construct(Camera camera, FrustumSelector frustumSelector,
            SelectedColonists selectedColonists, SelectedEnemies selectedEnemies, SelectedEntities selectedEntities)
        {
            _camera = camera;
            _frustumSelector = frustumSelector;

            _selectedColonists = selectedColonists;
            _selectedEnemies = selectedEnemies;
            _selectedEntities = selectedEntities;
        }

        private void OnEnable()
        {
            _frustumSelector.Selected += OnSelected;
        }

        private void OnDisable()
        {
            _frustumSelector.Selected -= OnSelected;
        }

        public void SelectFromRect(Rect rect)
        {
            _frustumSelector.Select(rect);
        }

        public void SelectFromPoint(Vector2 point)
        {
            DeselectAll();

            if (TrySelectColonistsFromPoint(point))
            {
                return;
            }

            if (TrySelectEnemiesFromPoint(point))
            {
                return;
            }

            SelectEntitiesFromPoint(point);
        }

        private void OnSelected(List<Collider> colliders)
        {
            DeselectAll();
            
            GetSelectedByType(colliders);

            if (_colonists.Count > 0)
            {
                _selectedColonists.Set(_colonists);
                return;
            }

            if (_enemies.Count > 0)
            {
                _selectedEnemies.Set(_enemies);
                return;
            }

            if (_entities.Count > 0)
            {
                _selectedEntities.Set(_entities);
            }
        }

        private void GetSelectedByType(List<Collider> colliders)
        {
            _colonists.Clear();
            _enemies.Clear();
            _entities.Clear();

            foreach (var collider in colliders)
            {
                if (collider.TryGetComponent(out Unit unit))
                {
                    if (unit.Fraction == Fraction.Colonists)
                    {
                        _colonists.Add(unit.Colonist);
                    }
                    else
                    {
                        _enemies.Add(unit.Enemy);
                    }
                    continue;
                }

                if (collider.TryGetComponent(out Entity entity))
                {
                    _entities.Add(entity);
                }
            }
        }

        private bool TrySelectColonistsFromPoint(Vector2 point)
        {
            var ray = _camera.ScreenPointToRay(point);

            if (Physics.Raycast(ray, out var hit))
            {
                if (hit.transform.TryGetComponent(out Colonist colonist) && colonist.Alive)
                {
                    _selectedColonists.Set(colonist);
                    return true;
                }
            }

            return false;
        }

        private bool TrySelectEnemiesFromPoint(Vector2 point)
        {
            var ray = _camera.ScreenPointToRay(point);

            if (Physics.Raycast(ray, out var hit))
            {
                if (hit.transform.TryGetComponent(out Enemy enemy) && enemy.Alive)
                {
                    _selectedEnemies.Set(enemy);
                    return true;
                }
            }

            return false;
        }

        private void SelectEntitiesFromPoint(Vector2 point)
        {
            var ray = _camera.ScreenPointToRay(point);

            if (Physics.Raycast(ray, out var hit))
            {
                if (hit.transform.TryGetComponent(out Entity entity) && entity.Alive)
                {
                    _selectedEntities.Set(entity);
                }
            }
        }

        private void DeselectAll()
        {
            _selectedColonists.Deselect();
            _selectedEnemies.Deselect();
            _selectedEntities.Deselect();
        }
    }
}
