using System;
using ResourceManagement;
using Sirenix.OdinInspector;
using Units;
using Units.Humans;
using UnityEngine;

namespace Colonists
{
    [RequireComponent(typeof(UnitAnimator))]
    public class ColonistAnimator : MonoBehaviour
    {
        [Required]
        [SerializeField] private HumanAnimations _humanAnimations;

        [Required]
        [SerializeField] private Animator _animator;

        private UnitAnimator _unitAnimator;

        private readonly int _cuttingTrees = Animator.StringToHash("cuttingTrees");
        private readonly int _miningRocks = Animator.StringToHash("miningRocks");

        private void Awake()
        {
            _unitAnimator = GetComponent<UnitAnimator>();
        }

        public void Move()
        {
            _humanAnimations.Move();
        }

        public void Idle()
        {
            _humanAnimations.Idle();
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

        public void Die(Action died)
        {
            _humanAnimations.Die(died);
        }
    }
}
