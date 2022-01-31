using BehaviorDesigner.Runtime;
using BehaviorDesigner.Runtime.Tasks;
using Enemies.Enemy;
using Entities.Entity;
using UnityEngine;

namespace Units.Unit.BehaviorNodes
{
    public class SearchForEnemy : Action
    {
        public SharedEnemy Enemy;
        public SharedBool NewCommand;

        public float ViewRadius;

        private readonly int _enemiesMask = LayerMask.GetMask("Enemies");
        private readonly Collider[] _hits = new Collider[20];

        private float _shortestDistanceToEnemy;

        public override void OnStart()
        {
            Enemy.Value = null;
            _shortestDistanceToEnemy = float.PositiveInfinity;
        }

        public override TaskStatus OnUpdate()
        {
            TryToFind();

            if (Enemy.Value != null)
            {
                NewCommand.Value = true;
            }

            return TaskStatus.Success;
        }

        private void TryToFind()
        {
            var quantity = Physics.OverlapSphereNonAlloc(transform.position, ViewRadius, _hits, _enemiesMask);
            for (var i = 0; i < quantity; i++)
            {
                var hit = _hits[i];

                var enemy = hit.transform.GetComponent<EnemyFacade>();
                if (enemy != null)
                {
                    SetIfClosest(enemy);
                }
            }
        }

        private void SetIfClosest(EnemyFacade enemy)
        {
            var distanceToEntity = Vector3.Distance(transform.position, enemy.transform.position);

            if (distanceToEntity > _shortestDistanceToEnemy)
            {
                return;
            }

            _shortestDistanceToEnemy = distanceToEntity;
            Enemy.Value = enemy;
        }
    }
}
