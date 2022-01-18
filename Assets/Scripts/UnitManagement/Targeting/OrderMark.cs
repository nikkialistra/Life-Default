using System;
using System.Collections.Generic;
using Entities;
using Entities.Entity;
using UnityEngine;

namespace UnitManagement.Targeting
{
    public class OrderMark : MonoBehaviour
    {
        [SerializeField] private GameObject _targetIndicator;

        private List<IOrderable> _orderables = new();

        public bool AtEntity => Entity != null;
        public Entity Entity { get; private set; }
        public bool Empty => _orderables.Count == 0;
        public IEnumerable<IOrderable> Orderables => _orderables;

        public void Add(IOrderable orderable)
        {
            _orderables.Add(orderable);

            orderable.DestinationReach += OnDestinationReach;

            UpdateState();
        }

        public void Remove(IOrderable orderable)
        {
            if (!_orderables.Contains(orderable))
            {
                return;
            }

            _orderables.Remove(orderable);

            UpdateState();

            orderable.DestinationReach -= OnDestinationReach;
        }

        public void Clear()
        {
            foreach (var orderable in _orderables)
            {
                orderable.DestinationReach -= OnDestinationReach;
            }

            _orderables.Clear();
        }

        public void SetEntity(Entity entity)
        {
            Entity = entity;
        }

        public void ClearEntity()
        {
            Entity = null;
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

        private void OnDestinationReach(IOrderable orderable)
        {
            if (!_orderables.Contains(orderable))
            {
                throw new InvalidOperationException();
            }

            orderable.DestinationReach -= OnDestinationReach;

            _orderables.Remove(orderable);

            UpdateState();
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
    }
}
