using Pathfinding;
using Sirenix.OdinInspector;
using UnityEngine;

namespace MapGeneration
{
    public class TerrainObject : MonoBehaviour
    {
        [Required]
        [SerializeField] private Transform _object;

        [Required]
        [SerializeField] private GraphUpdateScene _graphUpdateScene;

        private void OnDestroy()
        {
            //_graphUpdateScene.Apply();
        }

        public void Rotate(Quaternion rotation)
        {
            _object.transform.rotation = rotation;
        }
    }
}
