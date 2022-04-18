using System;
using UnityEngine;

namespace Entities.Animations
{
    [Serializable]
    public class TreeAnimations : IAnimations
    {
        [SerializeField] private Transform _tree;
        [SerializeField] private Transform _rotatePoint;

        public void OnHit(Vector3 lumberjackPosition)
        {
            var axis = new Vector3(_tree.position.x - lumberjackPosition.x, 0,
                _tree.position.z - lumberjackPosition.z);

            _tree.RotateAround(_rotatePoint.position, axis, 20f);
        }

        public void OnDestroy(Vector3 lumberjackPosition)
        {
            
        }
    }
}
