using BehaviorDesigner.Runtime.Tasks;
using Entities.Entity;
using UnityEngine;

namespace Units.Unit.BehaviorNodes
{
    public class FindEnemy : Action
    {
        public SharedEntity Entity;

        public float ViewRadius;

        private readonly int _entityMask = LayerMask.GetMask("Enemies");
        private readonly Collider[] _hits = new Collider[20];

        private float _shortestDistanceToEnemy;

        public override void OnStart()
        {
            Entity.Value = null;
            _shortestDistanceToEnemy = float.PositiveInfinity;
        }

        public override TaskStatus OnUpdate()
        {
            TryToFind();

            return Entity.Value != null ? TaskStatus.Success : TaskStatus.Failure;
        }

        private void TryToFind()
        {
            var quantity = Physics.OverlapSphereNonAlloc(transform.position, ViewRadius, _hits, _entityMask);
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
            var distanceToEntity = Vector3.Distance(transform.position, entity.transform.position);

            if (distanceToEntity > _shortestDistanceToEnemy)
            {
                return;
            }

            _shortestDistanceToEnemy = distanceToEntity;
            Entity = entity;
        }
    }
}
