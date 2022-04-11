﻿using System;
using Entities.Creature;
using ResourceManagement;
using Sirenix.OdinInspector;
using UnityEngine;

namespace Colonists.Colonist
{
    [RequireComponent(typeof(EntityAnimator))]
    public class ColonistAnimator : MonoBehaviour
    {
        [Required]
        [SerializeField] private Animator _animator;

        private EntityAnimator _entityAnimator;

        private readonly int _cuttingWood = Animator.StringToHash("cuttingWood");
        private readonly int _miningRock = Animator.StringToHash("miningRock");
        private readonly int _attacking = Animator.StringToHash("attacking");

        private void Awake()
        {
            _entityAnimator = GetComponent<EntityAnimator>();
        }

        public void Move(bool value)
        {
            _entityAnimator.Move(value);
        }

        public void Gather(Resource resource)
        {
            var interactionType = resource.ResourceType switch
            {
                ResourceType.Wood => _cuttingWood,
                ResourceType.Stone => _miningRock,
                _ => throw new ArgumentOutOfRangeException()
            };

            _animator.SetBool(interactionType, true);
        }

        public void StopGathering()
        {
            _animator.SetBool(_cuttingWood, false);
            _animator.SetBool(_miningRock, false);
        }
        
        public void Attack(bool value)
        {
            _animator.SetBool(_attacking, value);
        }

        public void Die(Action died)
        {
            _entityAnimator.Die(died);
        }
    }
}
