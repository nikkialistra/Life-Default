using System;
using System.Collections.Generic;
using System.Linq;
using Kernel.Types;
using UnityEngine;
using Zenject;

namespace Kernel.Targeting
{
    public class TargetObjectPool : MonoBehaviour
    {
        private GameObject _targetTemplate;
        private Transform _targetParent;

        private readonly Dictionary<GameObject, List<ITargetable>> _links = new();

        [Inject]
        public void Construct(GameObject targetTemplate, Transform targetParent)
        {
            _targetParent = targetParent;
            _targetTemplate = targetTemplate;
        }

        public GameObject PlaceTo(Vector3 position)
        {
            var target = GetFromPullOrCreate();

            target.transform.position = position;
            target.gameObject.SetActive(true);

            return target;
        }

        public void Link(GameObject point, ITargetable target)
        {
            if (!_links.ContainsKey(point))
            {
                throw new InvalidOperationException();
            }

            RemoveFromOldLink(target);
            
            _links[point].Add(target);

            OffAllWithoutLinks();
        }

        private void RemoveFromOldLink(ITargetable from)
        {
            _links.Values
                .FirstOrDefault(sources => sources.Contains(from))
                ?.Remove(from);
        }

        public void OffAll()
        {
            foreach (var point in _links.Keys)
                point.gameObject.SetActive(false);
        }

        private GameObject GetFromPullOrCreate()
        {
            foreach (var target in _links.Keys.Where(target => !_links[target].Any()))
            {
                return target;
            }

            return CreateNew();
        }

        private GameObject CreateNew()
        {
            var target = Instantiate(_targetTemplate, Vector3.zero, Quaternion.identity, _targetParent);
            target.SetActive(false);
            
            _links.Add(target, new List<ITargetable>());

            return target;
        }

        private void OffAllWithoutLinks()
        {
            foreach (var point in _links.Keys.Where(point => !_links[point].Any()))
            {
                point.gameObject.SetActive(false);
            }
        }
    }
}