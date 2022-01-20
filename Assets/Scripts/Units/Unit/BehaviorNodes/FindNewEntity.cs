using NPBehave;
using ResourceManagement;
using UnityEngine;

namespace Units.Unit.BehaviorNodes
{
    public class FindNewEntity : Node
    {
        private readonly int _entityMask = LayerMask.GetMask("Objects");

        private readonly Transform _transform;
        private readonly float _seekRadius;
        private readonly string _entityKey;
        private readonly Collider[] _hits = new Collider[20];

        public FindNewEntity(Transform transform, float seekRadius, string entityKey) : base("FindNewTarget")
        {
            _transform = transform;
            _seekRadius = seekRadius;
            _entityKey = entityKey;
        }

        protected override void DoStart()
        {
            Physics.OverlapSphereNonAlloc(_transform.position, _seekRadius, _hits, _entityMask);
            foreach (var hit in _hits)
            {
                var resource = hit.transform.GetComponentInParent<Resource>();
                if (resource != null && !resource.Exausted)
                {
                    Blackboard.Set(_entityKey, resource.Entity);

                    Stopped(true);
                    return;
                }
            }

            Stopped(false);
        }

        protected override void DoStop()
        {
            Stopped(false);
        }
    }
}
