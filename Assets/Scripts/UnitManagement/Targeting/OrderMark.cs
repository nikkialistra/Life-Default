using System;
using System.Collections.Generic;
using Entities.Entity;
using Units.Unit;
using UnityEngine;

namespace UnitManagement.Targeting
{
    public class OrderMark : MonoBehaviour
    {
        [SerializeField] private GameObject _targetIndicator;

        private List<UnitFacade> _units = new();

        public bool AtEntity => Entity != null;
        public Entity Entity { get; private set; }
        public bool Empty => _units.Count == 0;
        public IEnumerable<UnitFacade> Units => _units;

        public void Add(UnitFacade unit)
        {
            _units.Add(unit);

            unit.DestinationReach += OnDestinationReach;

            UpdateState();
        }

        public void Remove(UnitFacade unit)
        {
            if (!_units.Contains(unit))
            {
                return;
            }

            _units.Remove(unit);

            UpdateState();

            unit.DestinationReach -= OnDestinationReach;
        }

        public void Clear()
        {
            foreach (var unit in _units)
            {
                unit.DestinationReach -= OnDestinationReach;
            }

            _units.Clear();
        }

        public void SetEntity(Entity entity)
        {
            Entity = entity;
        }

        public void ClearEntity()
        {
            Entity = null;
        }

        private void Activate()
        {
            if (Entity != null)
            {
                Entity.ShowIndicator();
            }
            else
            {
                _targetIndicator.SetActive(true);
            }
        }

        public void Deactivate()
        {
            if (Entity != null)
            {
                HideTargetIndicator();
            }
            else
            {
                _targetIndicator.SetActive(false);
            }
        }

        private void HideTargetIndicator()
        {
            Entity.HideIndicator();
        }

        private void UpdateState()
        {
            if (!Empty)
            {
                Activate();
            }
            else
            {
                Deactivate();
            }
        }

        private void OnDestinationReach(UnitFacade unit)
        {
            if (!_units.Contains(unit))
            {
                throw new InvalidOperationException();
            }

            unit.DestinationReach -= OnDestinationReach;

            _units.Remove(unit);

            UpdateState();
        }
    }
}
