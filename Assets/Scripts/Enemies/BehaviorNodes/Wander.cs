using BehaviorDesigner.Runtime.Tasks;
using UnityEngine;

namespace Enemies.BehaviorNodes
{
    public class Wander : Action
    {
        public float MinWanderDistance = 5;
        public float MaxWanderDistance = 10;

        public float WanderRate = 3;

        public float MinPauseDuration = 0;
        public float MaxPauseDuration = 0;

        public EnemyMeshAgent EnemyMeshAgent;

        private float _setTargetTime = float.PositiveInfinity;

        public override void OnStart()
        {
            EnemyMeshAgent.StopMoving();
            if (MaxPauseDuration < MinPauseDuration)
            {
                MaxPauseDuration = MinPauseDuration;
            }
        }

        public override TaskStatus OnUpdate()
        {
            if (EnemyMeshAgent.Idle)
            {
                if (MaxPauseDuration > 0)
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

            _setTargetTime = Time.time + Random.Range(MinPauseDuration, MaxPauseDuration);
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
            var randomDirection = Random.insideUnitCircle * WanderRate;
            var wanderDirection = transform.forward + new Vector3(randomDirection.x, 0, randomDirection.y);
            var destination = transform.position +
                              wanderDirection.normalized * Random.Range(MinWanderDistance, MaxWanderDistance);

            EnemyMeshAgent.GoToPosition(destination);
        }
    }
}
