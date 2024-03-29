﻿using System;
using System.Collections.Generic;
using Aborigines;
using Colonists;
using Entities;
using Entities.Types;
using Selecting.Selected;
using Selecting.Selected.Entities;
using UI.Game.GameLook.Components.Info;
using Units;
using Units.Enums;
using UnityEngine;
using Zenject;

namespace Selecting
{
    public class Selection : MonoBehaviour
    {
        private bool NothingSelected =>
            _selectedColonists.Count == 0 &&
            _selectedAborigines.Count == 0 &&
            _selectedEntities.Count == 0;

        private readonly List<Colonist> _colonists = new();
        private readonly List<Aborigine> _aborigines = new();
        private readonly List<Entity> _entities = new();

        private Camera _camera;

        private FrustumSelector _frustumSelector;
        private InfoPanelView _infoPanelView;

        private SelectedColonists _selectedColonists;
        private SelectedAborigines _selectedAborigines;
        private SelectedEntities _selectedEntities;

        private EntitySelection _entitySelection;

        private bool _cancelSelecting;

        [Inject]
        public void Construct(Camera camera, FrustumSelector frustumSelector, InfoPanelView infoPanelView,
            SelectedColonists selectedColonists, SelectedAborigines selectedAborigines, SelectedEntities selectedEntities)
        {
            _camera = camera;
            _frustumSelector = frustumSelector;
            _infoPanelView = infoPanelView;

            _selectedColonists = selectedColonists;
            _selectedAborigines = selectedAborigines;
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

            if (TrySelectColonist(point)) return;

            if (TrySelectAborigine(point)) return;

            if (TrySelectEntity(point)) return;

            _infoPanelView.Hide();
        }

        public void SelectFromPointAdditive(Vector2 point)
        {
            if (_selectedColonists.Count > 0)
            {
                SelectColonistAdditive(point);
                return;
            }

            if (_selectedAborigines.Count > 0)
            {
                SelectAborigineAdditive(point);
                return;
            }

            if (_selectedEntities.Count > 0)
            {
                SelectEntityAdditive(point);
                return;
            }

            SelectFromPoint(point);
        }

        public void SelectFromAreaAround()
        {
            _frustumSelector.SelectFromAreaAround();
        }

        public void CancelSelecting()
        {
            _cancelSelecting = true;
        }

        private bool ShouldCancel()
        {
            if (_cancelSelecting)
            {
                _cancelSelecting = false;
                return true;
            }

            return false;
        }

        private void OnSelected(List<Collider> colliders)
        {
            if (ShouldCancel())
            {
                DeselectAllWithoutColonists();
                return;
            }

            DeselectAll();

            SplitByType(colliders);

            if (_colonists.Count > 0)
            {
                _selectedColonists.Set(_colonists);
                return;
            }

            if (_aborigines.Count > 0)
            {
                _selectedAborigines.Set(_aborigines);
                return;
            }

            if (_entities.Count > 0)
            {
                _entitySelection.ChooseToSelect(_entities);
                return;
            }

            _infoPanelView.Hide();
        }

        private void OnAdditiveSelected(List<Collider> colliders)
        {
            if (ShouldCancel())
            {
                DeselectAllWithoutColonists();
                return;
            }

            if (NothingSelected)
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

            if (_selectedAborigines.Count > 0 && _aborigines.Count > 0)
            {
                _selectedAborigines.Add(_aborigines);
                return;
            }

            if (_entities.Count > 0)
            {
                _entitySelection.ChooseToSelectAdditive(_entities);
                return;
            }

            OnSelected(colliders);
        }

        private void SplitByType(List<Collider> colliders)
        {
            _colonists.Clear();
            _aborigines.Clear();
            _entities.Clear();

            foreach (var collider in colliders)
            {
                if (collider.TryGetComponent(out Unit unit))
                {
                    if (unit.Faction == Faction.Colonists)
                        AddIfAlive(unit.Colonist);
                    else
                        AddIfAlive(unit.Aborigine);
                    continue;
                }

                if (collider.TryGetComponent(out Entity entity))
                    AddIfAlive(entity);
            }
        }

        private void AddIfAlive(Colonist entity)
        {
            if (entity.Alive)
                _colonists.Add(entity);
        }

        private void AddIfAlive(Aborigine aborigine)
        {
            if (aborigine.Alive)
                _aborigines.Add(aborigine);
        }

        private void AddIfAlive(Entity entity)
        {
            if (entity.Alive)
                _entities.Add(entity);
        }

        private bool TrySelectColonist(Vector2 point)
        {
            if (TryRaycastFromCamera(point, out var hit))
                if (hit.transform.TryGetComponent(out Colonist colonist) && colonist.Alive)
                {
                    _selectedColonists.Set(colonist);
                    return true;
                }

            return false;
        }

        private bool TrySelectAborigine(Vector2 point)
        {
            if (TryRaycastFromCamera(point, out var hit))
                if (hit.transform.TryGetComponent(out Aborigine aborigine) && aborigine.Alive)
                {
                    _selectedAborigines.Set(aborigine);
                    return true;
                }

            return false;
        }

        private bool TrySelectEntity(Vector2 point)
        {
            if (TryRaycastFromCamera(point, out var hit))
                if (hit.transform.TryGetComponent(out Entity entity) && entity.Alive)
                {
                    SelectByType(entity);
                    return true;
                }

            return false;
        }

        private void SelectColonistAdditive(Vector2 point)
        {
            if (TryRaycastFromCamera(point, out var hit))
                if (hit.transform.TryGetComponent(out Colonist colonist) && colonist.Alive)
                    _selectedColonists.Add(colonist);
        }

        private void SelectAborigineAdditive(Vector2 point)
        {
            if (TryRaycastFromCamera(point, out var hit))
                if (hit.transform.TryGetComponent(out Aborigine aborigine) && aborigine.Alive)
                    _selectedAborigines.Add(aborigine);
        }

        private void SelectEntityAdditive(Vector2 point)
        {
            if (TryRaycastFromCamera(point, out var hit))
                if (hit.transform.TryGetComponent(out Entity entity) && entity.Alive)
                    _selectedEntities.AddIfSameTypes(entity);
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
            _selectedAborigines.Deselect();
            _selectedEntities.Deselect();
        }

        private void DeselectAllWithoutColonists()
        {
            _selectedAborigines.Deselect();
            _selectedEntities.Deselect();
        }
    }
}
