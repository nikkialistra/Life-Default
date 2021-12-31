using System;
using System.Collections.Generic;
using UnityEngine;

namespace UnitManagement.Targeting
{
    public class Target : MonoBehaviour
    {
        public bool Empty => _targetables.Count == 0;
        public IEnumerable<ITargetable> Targetables => _targetables;

        private List<ITargetable> _targetables = new();

        public void Add(ITargetable targetable)
        {
            _targetables.Add(targetable);

            targetable.TargetReach += OnTargetReach;

            UpdateState();
        }

        public void Remove(ITargetable targetable)
        {
            if (!_targetables.Contains(targetable))
            {
                return;
            }

            _targetables.Remove(targetable);
            
            UpdateState();

            targetable.TargetReach -= OnTargetReach;
        }

        public void Clear()
        {
            foreach (var targetable in _targetables)
            {
                targetable.TargetReach -= OnTargetReach;
            }
            _targetables.Clear();
        }

        private void OnTargetReach(ITargetable targetable)
        {
            if (!_targetables.Contains(targetable))
            {
                throw new InvalidOperationException();
            }

            targetable.TargetReach -= OnTargetReach;

            _targetables.Remove(targetable);

            UpdateState();
        }

        private void UpdateState()
        {
            if (!Empty)
            {
                gameObject.SetActive(true);
            }
            else
            {
                gameObject.SetActive(false);
            }
        }
    }
}