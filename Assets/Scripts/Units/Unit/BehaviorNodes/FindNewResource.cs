using BehaviorDesigner.Runtime.Tasks;
using Entities.Entity;
using ResourceManagement;
using UnityEngine;

namespace Units.Unit.BehaviorNodes
{
    public class FindNewResource : Action
    {
        public SharedResource Resource;

        public float SeekRadius;

        private readonly int _resourcesMask = LayerMask.GetMask("Resources");
        private readonly Collider[] _hits = new Collider[20];

        private Resource _oldResource;

        private float _shortestDistanceToEntity = float.PositiveInfinity;

        public override void OnStart()
        {
            _oldResource = Resource.Value;

            _shortestDistanceToEntity = float.PositiveInfinity;
            Resource.Value = null;
        }

        public override TaskStatus OnUpdate()
        {
            TryToFind();

            return Resource.Value != null ? TaskStatus.Success : TaskStatus.Failure;
        }

        private void TryToFind()
        {
            var quantity = Physics.OverlapSphereNonAlloc(transform.position, SeekRadius, _hits, _resourcesMask);
            for (var i = 0; i < quantity; i++)
            {
                var hit = _hits[i];

                var resource = hit.transform.GetComponent<Resource>();
                if (resource != null && resource != _oldResource)
                {
                    SetIfSuitable(resource);
                }
            }
        }

        private void SetIfSuitable(Resource resource)
        {
            if (resource.ResourceType != _oldResource.ResourceType)
            {
                return;
            }

            SetIfClosest(resource);
        }

        private void SetIfClosest(Resource resource)
        {
            var distanceToResource = Vector3.Distance(transform.position, resource.transform.position);

            if (distanceToResource > _shortestDistanceToEntity)
            {
                return;
            }

            _shortestDistanceToEntity = distanceToResource;
            Resource.Value = resource;
        }
    }
}
