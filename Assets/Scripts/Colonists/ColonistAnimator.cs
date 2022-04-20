using System;
using ResourceManagement;
using Sirenix.OdinInspector;
using Units;
using UnityEngine;

namespace Colonists
{
    [RequireComponent(typeof(UnitAnimator))]
    public class ColonistAnimator : MonoBehaviour
    {
        [Required]
        [SerializeField] private Animator _animator;

        private UnitAnimator _unitAnimator;

        private readonly int _cuttingTrees = Animator.StringToHash("cuttingTrees");
        private readonly int _miningRocks = Animator.StringToHash("miningRocks");
        private readonly int _attacking = Animator.StringToHash("attacking");

        private void Awake()
        {
            _unitAnimator = GetComponent<UnitAnimator>();
        }

        public void Move(bool value)
        {
            _unitAnimator.Move(value);
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
            _unitAnimator.Die(died);
        }
    }
}
