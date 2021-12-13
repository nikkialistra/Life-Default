using System;
using System.Collections.Generic;
using System.Linq;
using Kernel.Types;
using UnityEngine;
using Zenject;

namespace Kernel.Targeting
{
    public class PointObjectPool : MonoBehaviour
    {
        private GameObject _template;
        
        private readonly Dictionary<GameObject, List<ITargetable>> _links = new Dictionary<GameObject, List<ITargetable>>();
        
        [Inject]
        public void Construct(GameObject template) => _template = template;

        public GameObject PlaceTo(Vector3 coordinate)
        {
            var target = GetFromPullOrCreate();

            target.transform.position = coordinate;
            target.gameObject.SetActive(true);

            return target;
        }

        public void Link(GameObject point, ITargetable from)
        {
            if (!_links.ContainsKey(point))
                throw new InvalidOperationException();

            _links.Values
                .FirstOrDefault(sources => sources.Contains(from))
                ?.Remove(from);
            
            _links[point].Add(from);

            OffAllWithoutLinks();
        }

        public void OffAll()
        {
            foreach (var point in _links.Keys)
                point.gameObject.SetActive(false);
        }

        private GameObject GetFromPullOrCreate()
        {
            foreach (var target in _links.Keys.Where(target => !_links[target].Any()))
                return target;

            return CreateNew();
        }

        private GameObject CreateNew()
        {
            var target = Instantiate(_template, Vector3.zero, Quaternion.identity);
            target.SetActive(false);
            
            _links.Add(target, new List<ITargetable>());

            return target;
        }

        private void OffAllWithoutLinks()
        {
            foreach (var point in _links.Keys.Where(point => !_links[point].Any()))
                point.gameObject.SetActive(false);
        }
    }
}