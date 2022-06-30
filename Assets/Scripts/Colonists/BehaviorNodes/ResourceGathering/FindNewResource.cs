using BehaviorDesigner.Runtime.Tasks;
using ResourceManagement;
using Units.Ancillaries.Fields;
using Units.BehaviorVariables;
using UnityEngine;

namespace Colonists.BehaviorNodes.ResourceGathering
{
    public class FindNewResource : Action
    {
        public FieldOfView FieldOfView;

        public SharedResource Resource;

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
            foreach (var target in FieldOfView.FindVisibleTargets())
            {
                var resource = target.GetComponent<Resource>();
                if (resource != null && resource != _oldResource)
                    SetIfSuitable(resource);
            }
        }

        private void SetIfSuitable(Resource resource)
        {
            if (resource.ResourceType != _oldResource.ResourceType) return;

            SetIfClosest(resource);
        }

        private void SetIfClosest(Resource resource)
        {
            var distanceToResource = Vector3.Distance(transform.position, resource.transform.position);

            if (distanceToResource > _shortestDistanceToEntity) return;

            _shortestDistanceToEntity = distanceToResource;
            Resource.Value = resource;
        }
    }
}
