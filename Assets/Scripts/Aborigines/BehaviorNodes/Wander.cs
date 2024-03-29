﻿using BehaviorDesigner.Runtime.Tasks;
using Units;
using UnityEngine;

namespace Aborigines.BehaviorNodes
{
    public class Wander : Action
    {
        public float MinWanderDistance = 5;
        public float MaxWanderDistance = 10;

        public float WanderRate = 3;

        public float MinPauseDuration = 0;
        public float MaxPauseDuration = 0;

        public AborigineMeshAgent AborigineMeshAgent;
        public UnitAttacker UnitAttacker;

        private float _setTargetTime = float.PositiveInfinity;

        private bool _destinationReached;

        public override void OnStart()
        {
            AborigineMeshAgent.StopMoving();

            if (MaxPauseDuration < MinPauseDuration)
                MaxPauseDuration = MinPauseDuration;
        }

        public override TaskStatus OnUpdate()
        {
            WanderIfIdle();

            return TaskStatus.Running;
        }

        private void WanderIfIdle()
        {
            if (AborigineMeshAgent.Idle && UnitAttacker.Idle)
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
        }

        private void InitPause()
        {
            if (!float.IsPositiveInfinity(_setTargetTime)) return;

            _setTargetTime = Time.time + Random.Range(MinPauseDuration, MaxPauseDuration);
        }

        private void SetTargetAfterPause()
        {
            if (_setTargetTime > Time.time) return;

            _setTargetTime = float.PositiveInfinity;
            SetTarget();
        }

        private void SetTarget()
        {
            var randomDirection = Random.insideUnitCircle * WanderRate;
            var wanderDirection = transform.forward + new Vector3(randomDirection.x, 0, randomDirection.y);
            var destination = transform.position +
                              wanderDirection.normalized * Random.Range(MinWanderDistance, MaxWanderDistance);

            AborigineMeshAgent.GoToPosition(destination);
        }
    }
}
