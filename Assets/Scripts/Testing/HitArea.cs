using Entities;
using Sirenix.OdinInspector;
using UnityEngine;

namespace Testing
{
    public class HitArea : MonoBehaviour, IHittable
    {
        [MinValue(0)]
        [SerializeField] private int _damage;
        [MinValue(0)]
        [SerializeField] private float _interval;
        public int Damage => _damage;
        public float Interval => _interval;
    }
}