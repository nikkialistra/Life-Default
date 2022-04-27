using System;
using System.Collections.Generic;
using System.Linq;
using Colonists;
using Enemies;
using Entities;
using Entities.Types;
using General.Selecting.Selected;
using Units;
using Units.Enums;
using UnityEngine;
using Zenject;

namespace General.Selecting
{
    public class Selection : MonoBehaviour
    {
        private Camera _camera;

        private readonly List<Colonist> _colonists = new();
        private readonly List<Enemy> _enemies = new();
        private readonly List<Entity> _entities = new();

        private SelectedColonists _selectedColonists;
        private SelectedEnemies _selectedEnemies;
        private SelectedEntities _selectedEntities;
        private FrustumSelector _frustumSelector;
        
        private EntitySelection _entitySelection;

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

        private void Awake()
        {
            _entitySelection = new EntitySelection(_selectedEntities);
        }

        private void OnEnable()
        {
            _frustumSelector.Selected += OnSelected;
            _frustumSelector.AdditiveSelected += OnAdditiveSelected;
        }

        private void OnDisable()
        {
            _frustumSelector.Selected -= OnSelected;
            _frustumSelector.AdditiveSelected -= OnAdditiveSelected;
        }

        public void SelectFromRect(Rect rect)
        {
            _frustumSelector.Select(rect);
        }

        public void SelectFromRectAdditive(Rect rect)
        {
            _frustumSelector.SelectAdditive(rect);
        }

        public void SelectFromPoint(Vector2 point)
        {
            DeselectAll();

            if (TrySelectColonist(point))
            {
                return;
            }

            if (TrySelectEnemy(point))
            {
                return;
            }

            SelectEntity(point);
        }

        public void SelectFromPointAdditive(Vector2 point)
        {
            if (_selectedColonists.Count > 0)
            {
                SelectColonistAdditive(point);
            }

            if (_selectedEnemies.Count > 0)
            {
                SelectEnemyAdditive(point);
            }

            SelectEntityAdditive(point);
        }

        private void OnSelected(List<Collider> colliders)
        {
            DeselectAll();
            
            SplitByType(colliders);

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
                _entitySelection.ChooseToSelect(_entities);
            }
        }

        private void OnAdditiveSelected(List<Collider> colliders)
        {
            if (NothingSelected())
            {
                OnSelected(colliders);
                return;
            }
            
            SplitByType(colliders);

            if (_selectedColonists.Count > 0 && _colonists.Count > 0)
            {
                _selectedColonists.Add(_colonists);
                return;
            }

            if (_selectedEnemies.Count > 0 && _enemies.Count > 0)
            {
                _selectedEnemies.Add(_enemies);
                return;
            }
            
            if (_entities.Count > 0)
            {
                _entitySelection.ChooseToSelectAdditive(_entities);
            }
        }

        private bool NothingSelected()
        {
            return _selectedColonists.Count == 0 &&
                   _selectedEnemies.Count == 0 &&
                   _selectedEntities.Count == 0;
        }

        private void SplitByType(List<Collider> colliders)
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
                        AddIfAlive(unit.Colonist);
                    }
                    else
                    {
                        AddIfAlive(unit.Enemy);
                    }
                    continue;
                }

                if (collider.TryGetComponent(out Entity entity))
                {
                    AddIfAlive(entity);
                }
            }
        }

        private void AddIfAlive(Colonist entity)
        {
            if (entity.Alive)
            {
                _colonists.Add(entity);
            }
        }

        private void AddIfAlive(Enemy enemy)
        {
            if (enemy.Alive)
            {
                _enemies.Add(enemy);
            }
        }

        private void AddIfAlive(Entity entity)
        {
            if (entity.Alive)
            {
                _entities.Add(entity);
            }
        }

        private bool TrySelectColonist(Vector2 point)
        {
            if (TryRaycastFromCamera(point, out var hit))
            {
                if (hit.transform.TryGetComponent(out Colonist colonist) && colonist.Alive)
                {
                    _selectedColonists.Set(colonist);
                    return true;
                }
            }
            
            return false;
        }

        private bool TrySelectEnemy(Vector2 point)
        {
            if (TryRaycastFromCamera(point, out var hit))
            {
                if (hit.transform.TryGetComponent(out Enemy enemy) && enemy.Alive)
                {
                    _selectedEnemies.Set(enemy);
                    return true;
                }
            }

            return false;
        }

        private void SelectEntity(Vector2 point)
        {
            if (TryRaycastFromCamera(point, out var hit))
            {
                if (hit.transform.TryGetComponent(out Entity entity) && entity.Alive)
                {
                    SelectByType(entity);
                }
            }
        }

        private void SelectColonistAdditive(Vector2 point)
        {
            if (TryRaycastFromCamera(point, out var hit))
            {
                if (hit.transform.TryGetComponent(out Colonist colonist) && colonist.Alive)
                {
                    _selectedColonists.Add(colonist);
                }
            }
        }
        
        private void SelectEnemyAdditive(Vector2 point)
        {
            if (TryRaycastFromCamera(point, out var hit))
            {
                if (hit.transform.TryGetComponent(out Enemy enemy) && enemy.Alive)
                {
                    _selectedEnemies.Add(enemy);
                }
            }
        }
        
        private void SelectEntityAdditive(Vector2 point)
        {
            if (TryRaycastFromCamera(point, out var hit))
            {
                if (hit.transform.TryGetComponent(out Entity entity) && entity.Alive)
                {
                    _selectedEntities.AddIfSameTypes(entity);
                }
            }
        }

        private bool TryRaycastFromCamera(Vector2 point, out RaycastHit hitInfo)
        {
            var ray = _camera.ScreenPointToRay(point);

            if (Physics.Raycast(ray, out var hit))
            {
                hitInfo = hit;
                return true;
            }

            hitInfo = new RaycastHit();
            return false;
        }

        private void SelectByType(Entity entity)
        {
            switch (entity.EntityType)
            {
                case EntityType.Resource:
                    _selectedEntities.Set(entity.Resource);
                    break;
                case EntityType.ResourceChunk:
                    _selectedEntities.Set(entity.ResourceChunk);
                    break;
                case EntityType.Building:
                    break;
                default:
                    throw new ArgumentOutOfRangeException();
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
