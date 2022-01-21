using MapGeneration.Generators;
using Sirenix.OdinInspector;
using UnityEngine;

namespace MapGeneration
{
    [RequireComponent(typeof(Collider))]
    public class TerrainObject : MonoBehaviour
    {
        [Required]
        [SerializeField] private Transform _object;

        private Collider _collider;

        private TerrainObjectGenerator _generator;

        private void Awake()
        {
            _collider = GetComponent<Collider>();
        }

        public void UpdateGraph()
        {
            var bounds = _collider.bounds;
            _generator.UpdateGraph(bounds);
        }

        public void Initialize(TerrainObjectGenerator generator)
        {
            _generator = generator;
        }

        public void Rotate(Quaternion rotation)
        {
            _object.transform.rotation = rotation;
        }
    }
}
