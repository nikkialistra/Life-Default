using System;
using System.Collections.Generic;
using UnityEngine;

namespace UnitManagement.Targeting
{
    public class Target : MonoBehaviour
    {
        [SerializeField] private GameObject _targetIndicator;

        private List<ITargetable> _targetables = new();
        private TargetObject _targetObject;

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

        public void SetTargetObject(TargetObject targetObject)
        {
            _targetObject = targetObject;
        }

        public void Deactivate()
        {
            if (_targetObject != null)
            {
                RemoveTargetObject();
            }
            else
            {
                _targetIndicator.SetActive(false);
            }
        }

        private void RemoveTargetObject()
        {
            if (_targetObject.HasDestinationPoint)
            {
                _targetObject.HideIndicator();
            }

            _targetObject = null;
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
            if (_targetObject != null && _targetObject.HasDestinationPoint)
            {
                _targetObject.ShowIndicator();
            }
            else
            {
                _targetIndicator.SetActive(true);
            }
        }
    }
}