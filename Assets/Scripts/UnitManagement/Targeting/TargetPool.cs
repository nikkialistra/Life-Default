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
        private GameObject _targetTemplate;
        private Transform _targetParent;

        private readonly Dictionary<GameObject, List<ITargetable>> _links = new();
        
        private UnitsRepository _unitsRepository;

        [Inject]
        public void Construct(GameObject targetTemplate, Transform targetParent, UnitsRepository unitsRepository)
        {
            _targetTemplate = targetTemplate;
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

        public GameObject PlaceTo(Vector3 position)
        {
            var target = GetFromPoolOrCreate();

            target.transform.position = position;
            target.gameObject.SetActive(false);

            return target;
        }

        public void Link(GameObject target, ITargetable targetable)
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

        private GameObject GetFromPoolOrCreate()
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

        private void AddTarget(GameObject target, ITargetable targetable)
        {
            _links[target].Add(targetable);
            targetable.TargetReach += OnTargetReach;
        }

        private void OnTargetReach(ITargetable targetable, GameObject target)
        {
            if (!_links[target].Contains(targetable))
            {
                throw new InvalidOperationException();
            }

            _links[target].Remove(targetable);
            
            targetable.TargetReach -= OnTargetReach;

            UpdateTargetShowing();
        }

        private GameObject CreateNew()
        {
            var target = Instantiate(_targetTemplate, Vector3.zero, Quaternion.identity, _targetParent);
            target.SetActive(false);
            
            _links.Add(target, new List<ITargetable>());

            return target;
        }

        private void UpdateTargetShowing()
        {
            foreach (var target in _links.Keys)
            {
                target.SetActive(_links[target].Any());
            }
        }
    }
}