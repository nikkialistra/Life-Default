using Sirenix.OdinInspector;
using UnityEngine;

namespace MapGeneration
{
    [RequireComponent(typeof(Collider))]
    public class TerrainObject : MonoBehaviour
    {
        [Required]
        [SerializeField] private Transform _object;

        public void Rotate(Quaternion rotation)
        {
            _object.transform.rotation = rotation;
        }
    }
}
