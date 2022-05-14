using System;
using ResourceManagement;
using Units;
using UnityEngine;

namespace Colonists
{
    [RequireComponent(typeof(UnitAnimator))]
    [RequireComponent(typeof(ColonistAnimations))]
    public class ColonistAnimator : MonoBehaviour
    { 
        private ColonistAnimations _colonistAnimations;
        private UnitAnimator _unitAnimator;

        private void Awake()
        {
            _colonistAnimations = GetComponent<ColonistAnimations>();
            _unitAnimator = GetComponent<UnitAnimator>();
        }

        public void Idle()
        {
            _unitAnimator.Idle();
        }

        public void Move()
        {
            _unitAnimator.Move();
        }

        public void Gather(Resource resource)
        {
            Action action = resource.ResourceType switch
            {
                ResourceType.Wood => _colonistAnimations.CutTrees,
                ResourceType.Stone => _colonistAnimations.MineRocks,
                _ => throw new ArgumentOutOfRangeException()
            };

            action();
        }

        public void StopGathering()
        {
            _colonistAnimations.StopGathering();
        }

        public void Die(Action died)
        {
            _unitAnimator.Die(died);
        }
    }
}
