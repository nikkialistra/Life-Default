using BehaviorDesigner.Runtime.Tasks;
using Entities.Entity;
using UnityEngine;

namespace Units.Unit.BehaviorNodes
{
    public class FindNewEntity : Action
    {
        public SharedEntity Entity;

        public float SeekRadius;

        private readonly int _entityMask = LayerMask.GetMask("Units", "Enemies", "Buildings", "Resources");
        private readonly Collider[] _hits = new Collider[20];

        private Entity _oldEntity;

        private float _shortestDistanceToEntity = float.PositiveInfinity;

        public override void OnStart()
        {
            _oldEntity = Entity.Value;

            _shortestDistanceToEntity = float.PositiveInfinity;
            Entity.Value = null;
        }

        public override TaskStatus OnUpdate()
        {
            TryToFind();

            return Entity.Value != null ? TaskStatus.Success : TaskStatus.Failure;
        }

        private void TryToFind()
        {
            var quantity = Physics.OverlapSphereNonAlloc(transform.position, SeekRadius, _hits, _entityMask);
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
            var distanceToEntity = Vector3.Distance(transform.position, entity.transform.position);

            if (distanceToEntity > _shortestDistanceToEntity)
            {
                return;
            }

            _shortestDistanceToEntity = distanceToEntity;
            Entity.Value = entity;
        }
    }
}
