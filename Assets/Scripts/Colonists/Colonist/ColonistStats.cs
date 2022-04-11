using UnityEngine;

namespace Colonists.Colonist
{
    public class ColonistStats : MonoBehaviour
    {
        [SerializeField] private float _resourceExtractionSpeed = 5f;
        [Range(0f, 1f)]
        [SerializeField] private float _resourceExtractionEfficiency = 0.86f;
        
        public float ResourceExtractionSpeed => _resourceExtractionSpeed;
        public float ResourceExtractionEfficiency => _resourceExtractionEfficiency;
    }
}
