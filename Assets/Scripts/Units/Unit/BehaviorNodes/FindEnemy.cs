using Entities.Entity;
using NPBehave;
using UnityEngine;

namespace Units.Unit.BehaviorNodes
{
    public class FindEnemy : Node
    {
        private readonly int _entityMask = LayerMask.GetMask("Enemies");

        private readonly string _entityKey;
        private readonly Transform _transform;
        private readonly float _viewRadius;

        private readonly Collider[] _hits = new Collider[20];

        private float _shortestDistanceToEntity;
        private Entity _newEntity;

        public FindEnemy(string entityKey, Transform transform, float viewRadius) : base("FindEnemy")
        {
            _entityKey = entityKey;
            _transform = transform;
            _viewRadius = viewRadius;
        }

        protected override void DoStart()
        {
            Find();

            if (_newEntity != null)
            {
                Blackboard.Set(_entityKey, _newEntity);
                ResetData();
                Stopped(false);
            }
            else
            {
                ResetData();
                Stopped(true);
            }
        }

        private void ResetData()
        {
            _shortestDistanceToEntity = float.PositiveInfinity;
            _newEntity = null;
        }

        private void Find()
        {
            var quantity = Physics.OverlapSphereNonAlloc(_transform.position, _viewRadius, _hits, _entityMask);
            for (var i = 0; i < quantity; i++)
            {
                var hit = _hits[i];

                var entity = hit.transform.GetComponent<Entity>();
                if (entity != null && entity.EntityType == EntityType.Enemy)
                {
                    SetIfClosest(entity);
                }
            }
        }

        private void SetIfClosest(Entity entity)
        {
            var distanceToEntity = Vector3.Distance(_transform.position, entity.transform.position);

            if (distanceToEntity > _shortestDistanceToEntity)
            {
                return;
            }

            _shortestDistanceToEntity = distanceToEntity;
            _newEntity = entity;
        }

        protected override void DoStop()
        {
            Stopped(false);
        }
    }
}
