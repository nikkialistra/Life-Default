using General.Interfaces;
using Sirenix.OdinInspector;
using UnityEngine;

namespace Testing
{
    public class HitArea : MonoBehaviour, IHittable
    {
        public float Damage => _damage;
        public float Interval => _interval;

        [MinValue(0)]
        [SerializeField] private float _damage;
        [MinValue(0)]
        [SerializeField] private float _interval;
    }
}
