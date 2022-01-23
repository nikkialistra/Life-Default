﻿using System;
using Entities.Entity;
using Sirenix.OdinInspector;
using UnityEngine;

namespace Units.Unit
{
    [RequireComponent(typeof(UnitMeshAgent))]
    [RequireComponent(typeof(EntityAnimator))]
    public class UnitAnimator : MonoBehaviour
    {
        [Required]
        [SerializeField] private Animator _animator;

        private UnitMeshAgent _unitMeshAgent;
        private EntityAnimator _entityAnimator;

        private readonly int _interactingWithResource = Animator.StringToHash("interactingWithResource");

        private void Awake()
        {
            _unitMeshAgent = GetComponent<UnitMeshAgent>();
            _entityAnimator = GetComponent<EntityAnimator>();
        }

        private void Update()
        {
            SetAnimatorVelocity();
        }

        public void InteractWithResource()
        {
            _animator.SetBool(_interactingWithResource, true);
        }

        public void StopInteractWithResource()
        {
            _animator.SetBool(_interactingWithResource, false);
        }

        public void Die(Action died)
        {
            _entityAnimator.Die(died);
        }

        private void SetAnimatorVelocity()
        {
            _entityAnimator.SetAnimatorVelocity(_unitMeshAgent.Velocity);
        }
    }
}
