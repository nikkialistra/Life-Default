using Entities.Entity;
using NPBehave;
using UnityEngine;

namespace Units.Unit.BehaviorNodes
{
    public class FindNewEntity : Node
    {
        private readonly int _entityMask = LayerMask.GetMask("Entities");

        private readonly Transform _transform;
        private readonly float _seekRadius;
        private readonly string _entityKey;
        private readonly Collider[] _hits = new Collider[20];

        private Entity _oldEntity;

        private float _shortestDistanceToEntity = float.PositiveInfinity;
        private bool _entityIsSet;

        public FindNewEntity(string entityKey, Transform transform, float seekRadius) : base("FindNewTarget")
        {
            _entityKey = entityKey;
            _transform = transform;
            _seekRadius = seekRadius;
        }

        protected override void DoStart()
        {
            if (!Blackboard.Isset(_entityKey))
            {
                Stopped(false);
                return;
            }

            _oldEntity = Blackboard.Get<Entity>(_entityKey);

            Blackboard.Unset(_entityKey);

            FindEntity();

            Stopped(_entityIsSet);
            ResetData();
        }

        private void ResetData()
        {
            _shortestDistanceToEntity = float.PositiveInfinity;
            _entityIsSet = false;
        }

        private void FindEntity()
        {
            var quantity = Physics.OverlapSphereNonAlloc(_transform.position, _seekRadius, _hits, _entityMask);
            for (var i = 0; i < quantity; i++)
            {
                var hit = _hits[i];

                var entity = hit.transform.GetComponent<Entity>();
                if (entity != null && entity != _oldEntity)
                {
                    SetIfSuitable(entity);
                }
            }
        }

        private void SetIfSuitable(Entity entity)
        {
            if (entity.EntityType == _oldEntity.EntityType)
            {
                if (CheckIfDifferentResources(entity))
                {
                    return;
                }

                SetIfClosest(entity);
            }
        }

        private bool CheckIfDifferentResources(Entity entity)
        {
            if (entity.EntityType == EntityType.Resource)
            {
                if (entity.Resource.ResourceType != _oldEntity.Resource.ResourceType)
                {
                    return true;
                }
            }

            return false;
        }

        private void SetIfClosest(Entity entity)
        {
            var distanceToEntity = Vector3.Distance(_transform.position, entity.transform.position);

            if (distanceToEntity > _shortestDistanceToEntity)
            {
                return;
            }

            _shortestDistanceToEntity = distanceToEntity;
            Blackboard.Set(_entityKey, entity);
            _entityIsSet = true;
        }

        protected override void DoStop()
        {
            Stopped(false);
        }
    }
}
