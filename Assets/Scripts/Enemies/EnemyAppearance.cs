using Sirenix.OdinInspector;
using UnityEngine;

namespace Enemies
{
    public class EnemyAppearance : MonoBehaviour
    {
        [Required]
        [SerializeField] private SkinnedMeshRenderer _skinnedMeshRenderer;

        [Required]
        [SerializeField] private Material _skin;

        private void Start()
        {
            SetSkin();
        }

        private void SetSkin()
        {
            var materials = _skinnedMeshRenderer.materials;
            materials[0] = _skin;
            _skinnedMeshRenderer.materials = materials;
        }
    }
}
