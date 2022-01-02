using System;
using System.Collections.Generic;
using Units.Services;
using Units.Unit;
using UnityEngine;
using Zenject;

namespace UnitManagement.Targeting
{
    public class TargetPool : MonoBehaviour
    {
        private Target _template;
        private Transform _targetParent;

        private readonly List<Target> _targets = new();

        private UnitsRepository _unitsRepository;

        [Inject]
        public void Construct(Target template, Transform targetParent, UnitsRepository unitsRepository)
        {
            _template = template;
            _targetParent = targetParent;
            _unitsRepository = unitsRepository;
        }

        private void OnEnable()
        {
            _unitsRepository.Remove += OnRemove;
        }

        private void OnDisable()
        {
            _unitsRepository.Remove -= OnRemove;
        }

        public Target PlaceTo(Vector3 position, TargetObject targetObject = null)
        {
            var target = GetFromPoolOrCreate();
            if (targetObject)
            {
                target.SetTargetObject(targetObject);
            }

            target.transform.position = position;

            return target;
        }

        public void Link(Target target, ITargetable targetable)
        {
            if (!_targets.Contains(target))
            {
                throw new InvalidOperationException();
            }

            RemoveFromOldTarget(targetable);
            AddTarget(target, targetable);
        }

        public void OffAll()
        {
            foreach (var target in _targets)
            {
                target.Clear();
                _targets.Remove(target);
                Destroy(target.gameObject);
            }
        }

        private void OnRemove(UnitFacade unit)
        {
            RemoveFromOldTarget(unit.Targetable);
        }

        private Target GetFromPoolOrCreate()
        {
            foreach (var target in _targets)
            {
                if (target.Empty)
                {
                    return target;
                }
            }

            return CreateNew();
        }

        private void RemoveFromOldTarget(ITargetable targetable)
        {
            foreach (var target in _targets)
            {
                target.Remove(targetable);
            }
        }

        private void AddTarget(Target target, ITargetable targetable)
        {
            _targets.Add(target);
            target.Add(targetable);
        }

        private Target CreateNew()
        {
            var target = Instantiate(_template, Vector3.zero, Quaternion.identity, _targetParent);
            target.Deactivate();

            _targets.Add(target);

            return target;
        }
    }
}