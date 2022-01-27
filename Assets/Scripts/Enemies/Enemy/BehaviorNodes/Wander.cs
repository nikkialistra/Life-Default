using BehaviorDesigner.Runtime.Tasks;
using UnityEngine;

namespace Enemies.Enemy.BehaviorNodes
{
    public class Wander : Action
    {
        public float _minWanderDistance = 5;
        public float _maxWanderDistance = 10;

        public float _wanderRate = 3;

        public float _minPauseDuration = 0;
        public float _maxPauseDuration = 0;

        public EnemyMeshAgent _enemyMeshAgent;

        private float _setTargetTime = float.PositiveInfinity;

        public override void OnStart()
        {
            if (_maxPauseDuration < _minPauseDuration)
            {
                _maxPauseDuration = _minPauseDuration;
            }
        }

        public override TaskStatus OnUpdate()
        {
            if (_enemyMeshAgent.Idle)
            {
                if (_maxPauseDuration > 0)
                {
                    InitPause();
                    SetTargetAfterPause();
                }
                else
                {
                    SetTarget();
                }
            }

            return TaskStatus.Running;
        }

        private void InitPause()
        {
            if (!float.IsPositiveInfinity(_setTargetTime))
            {
                return;
            }

            _setTargetTime = Time.time + Random.Range(_minPauseDuration, _maxPauseDuration);
        }

        private void SetTargetAfterPause()
        {
            if (_setTargetTime > Time.time)
            {
                return;
            }

            _setTargetTime = float.PositiveInfinity;
            SetTarget();
        }

        private void SetTarget()
        {
            var randomDirection = Random.insideUnitCircle * _wanderRate;
            var wanderDirection = transform.forward + new Vector3(randomDirection.x, 0, randomDirection.y);
            var destination = transform.position +
                              wanderDirection.normalized * Random.Range(_minWanderDistance, _maxWanderDistance);

            _enemyMeshAgent.GoToPosition(destination);
        }
    }
}
