using Sirenix.OdinInspector;
using UnityEngine;

namespace MapGeneration
{
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
