using UnityEngine;

namespace Entities.Animations
{
    public class TreeAnimations : IAnimations
    {
        [SerializeField] private Transform _tree;

        public void OnExtraction(Vector3 lumberjackPosition)
        {
            Debug.Log(1);
            
            var difference = new Vector3(_tree.position.x - lumberjackPosition.x, 0,
                _tree.position.z - lumberjackPosition.z);

            //transform.Rotate(difference, 20f);
        }

        public void OnExhaustion(Vector3 lumberjackPosition)
        {
            
        }
    }
}
