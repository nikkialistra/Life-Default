using System;
using System.Collections.Generic;
using UnityEngine;

namespace UnitManagement.Targeting
{
    public class TargetMark : MonoBehaviour
    {
        [SerializeField] private GameObject _targetIndicator;

        private List<ITargetable> _targetables = new();

        public bool HasTarget => Target != null;
        public Target Target { get; private set; }
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
            Target = target;
        }

        public void ClearTargetObject()
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
            if (Target.HasDestinationPoint)
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
            if (Target != null && Target.HasDestinationPoint)
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
