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
        
        private UnitAnimator _unitAnimator;

        private void Awake()
        {
            _unitAnimator = GetComponent<UnitAnimator>();
        }

        public void Idle()
        {
            _unitAnimator.Idle();
            _humanAnimations.Idle();
        }

        public void Move()
        {
            _unitAnimator.Move();
        }

        public void Gather(Resource resource)
        {
            Action action = resource.ResourceType switch
            {
                ResourceType.Wood => _humanAnimations.CutTrees,
                ResourceType.Stone => _humanAnimations.MineRocks,
                _ => throw new ArgumentOutOfRangeException()
            };

            action();
        }

        public void StopGathering()
        {
            _humanAnimations.StopGathering();
        }

        public void Die(Action died)
        {
            _unitAnimator.Die(died);
        }
    }
}
