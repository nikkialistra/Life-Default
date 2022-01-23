﻿using System;
using Entities.Entity;
using UnityEngine;

namespace Enemies.Enemy
{
    [RequireComponent(typeof(EntityAnimator))]
    public class EnemyAnimator : MonoBehaviour
    {
        private EnemyMeshAgent _enemyMeshAgent;
        private EntityAnimator _entityAnimator;

        private void Awake()
        {
            _enemyMeshAgent = GetComponent<EnemyMeshAgent>();
            _entityAnimator = GetComponent<EntityAnimator>();
        }

        private void Update()
        {
            SetAnimatorVelocity();
        }

        public void Die(Action died)
        {
            _entityAnimator.Die(died);
        }

        private void SetAnimatorVelocity()
        {
            _entityAnimator.SetAnimatorVelocity(_enemyMeshAgent.Velocity);
        }
    }
}
