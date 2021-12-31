using System;
using System.Collections.Generic;
using System.Linq;
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

        private readonly Dictionary<Target, List<ITargetable>> _links = new();
        
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

        public Target PlaceTo(Vector3 position)
        {
            var target = GetFromPoolOrCreate();

            target.transform.position = position;
            target.gameObject.SetActive(false);

            return target;
        }

        public void Link(Target target, ITargetable targetable)
        {
            if (!_links.ContainsKey(target))
            {
                throw new InvalidOperationException();
            }
            
            RemoveFromOldTarget(targetable);
            AddTarget(target, targetable);

            UpdateTargetShowing();
        }

        public void OffAll()
        {
            foreach (var target in _links.Keys)
            {
                foreach (var targetable in _links[target])
                {
                    targetable.TargetReach -= OnTargetReach;
                }
                _links[target].Clear();
                
                _links.Remove(target);
                Destroy(target.gameObject);
            }
        }

        private void OnRemove(UnitFacade unit)
        {
            RemoveFromOldTarget(unit.Targetable);
            UpdateTargetShowing();
        }

        private Target GetFromPoolOrCreate()
        {
            foreach (var target in _links.Keys)
            {
                if (!_links[target].Any())
                {
                    return target;
                }
            }

            return CreateNew();
        }

        private void RemoveFromOldTarget(ITargetable targetable)
        {
            var link = _links.Values
                .FirstOrDefault(target => target.Contains(targetable));

            if (link != null)
            {
                link.Remove(targetable);
                targetable.TargetReach -= OnTargetReach;
            }
        }

        private void AddTarget(Target target, ITargetable targetable)
        {
            _links[target].Add(targetable);
            targetable.TargetReach += OnTargetReach;
        }

        private void OnTargetReach(ITargetable targetable, Target target)
        {
            if (!_links[target].Contains(targetable))
            {
                throw new InvalidOperationException();
            }

            _links[target].Remove(targetable);
            
            targetable.TargetReach -= OnTargetReach;

            UpdateTargetShowing();
        }

        private Target CreateNew()
        {
            var target = Instantiate(_template, Vector3.zero, Quaternion.identity, _targetParent);
            target.gameObject.SetActive(false);
            
            _links.Add(target, new List<ITargetable>());

            return target;
        }

        private void UpdateTargetShowing()
        {
            foreach (var target in _links.Keys)
            {
                target.gameObject.SetActive(_links[target].Any());
            }
        }
    }
}