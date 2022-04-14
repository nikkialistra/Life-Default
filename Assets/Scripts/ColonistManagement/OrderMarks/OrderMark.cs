using System;
using System.Collections.Generic;
using Colonists.Colonist;
using Entities;
using UnityEngine;

namespace ColonistManagement.OrderMarks
{
    public class OrderMark : MonoBehaviour
    {
        private readonly List<ColonistFacade> _colonists = new();

        private Coroutine _collapseTargetIndicatorCoroutine;

        private bool _activated;

        public Entity Entity { get; set; }
        public bool Empty => _colonists.Count == 0;

        public void Add(ColonistFacade colonist)
        {
            _colonists.Add(colonist);

            colonist.DestinationReach += OnDestinationReach;

            UpdateState();
        }

        public void Remove(ColonistFacade colonist)
        {
            if (!_colonists.Contains(colonist))
            {
                return;
            }

            _colonists.Remove(colonist);

            UpdateState();

            colonist.DestinationReach -= OnDestinationReach;
        }

        public void Clear()
        {
            foreach (var colonist in _colonists)
            {
                colonist.DestinationReach -= OnDestinationReach;
            }

            _colonists.Clear();
        }

        private void Activate()
        {
            if (_activated)
            {
                return;
            }

            _activated = true;

            Entity?.Flash();
        }

        public void Deactivate()
        {
            _activated = false;
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

        private void OnDestinationReach(ColonistFacade colonist)
        {
            if (!_colonists.Contains(colonist))
            {
                throw new InvalidOperationException();
            }

            colonist.DestinationReach -= OnDestinationReach;

            _colonists.Remove(colonist);

            UpdateState();
        }
    }
}
