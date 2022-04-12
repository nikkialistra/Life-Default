using System;
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

        private readonly int _cuttingTrees = Animator.StringToHash("cuttingTrees");
        private readonly int _miningRocks = Animator.StringToHash("miningRocks");
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
                ResourceType.Wood => _cuttingTrees,
                ResourceType.Stone => _miningRocks,
                _ => throw new ArgumentOutOfRangeException()
            };

            _animator.SetBool(interactionType, true);
        }

        public void StopGathering()
        {
            _animator.SetBool(_cuttingTrees, false);
            _animator.SetBool(_miningRocks, false);
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
