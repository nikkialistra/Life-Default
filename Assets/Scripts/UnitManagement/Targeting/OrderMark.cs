using System;
using System.Collections.Generic;
using UnityEngine;

namespace UnitManagement.Targeting
{
    public class OrderMark : MonoBehaviour
    {
        [SerializeField] private GameObject _targetIndicator;

        private List<IOrderable> _orderables = new();

        public bool HasTarget => Target != null;
        public Target Target { get; private set; }
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

        public void SetTarget(Target target)
        {
            Target = target;
        }

        public void ClearTarget()
        {
            Target = null;
        }

        public void Deactivate()
        {
            if (Target != null)
            {
                HideTargetObjectIndicator();
            }
            else
            {
                _targetIndicator.SetActive(false);
            }
        }

        private void HideTargetObjectIndicator()
        {
            if (Target.IsEntity)
            {
                Target.HideIndicator();
            }
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
            if (Target != null && Target.IsEntity)
            {
                Target.ShowIndicator();
            }
            else
            {
                _targetIndicator.SetActive(true);
            }
        }
    }
}
