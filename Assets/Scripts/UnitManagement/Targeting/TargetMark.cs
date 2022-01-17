using System;
using System.Collections.Generic;
using UnityEngine;

namespace UnitManagement.Targeting
{
    public class TargetMark : MonoBehaviour
    {
        [SerializeField] private GameObject _targetIndicator;

        private List<ITargetable> _targetables = new();
        private Target _target;

        public bool HasTarget => _target != null;
        public bool Empty => _targetables.Count == 0;
        public IEnumerable<ITargetable> Targetables => _targetables;

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

        public void SetTargetObject(Target target)
        {
            _target = target;
        }

        public void ClearTargetObject()
        {
            _target = null;
        }

        public void Deactivate()
        {
            if (_target != null)
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
            if (_target.HasDestinationPoint)
            {
                _target.HideIndicator();
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

        private void Activate()
        {
            if (_target != null && _target.HasDestinationPoint)
            {
                _target.ShowIndicator();
            }
            else
            {
                _targetIndicator.SetActive(true);
            }
        }
    }
}
