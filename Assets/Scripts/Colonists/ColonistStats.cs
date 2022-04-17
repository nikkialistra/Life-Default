using UnityEngine;

namespace Colonists
{
    public class ColonistStats : MonoBehaviour
    {
        [SerializeField] private float _resourceDestructionSpeed = 5f;
        [Range(0f, 1f)]
        [SerializeField] private float _resourceExtractionEfficiency = 0.86f;
        
        public float ResourceDestructionSpeed => _resourceDestructionSpeed;
        public float ResourceExtractionEfficiency => _resourceExtractionEfficiency;
    }
}
