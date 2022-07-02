using System;
using UnityEngine;

namespace Units.Stats
{
    [Serializable]
    public class StatModifier<T>
    {
        public T StatType => _statType;
        public StatModifierType ModifierType => _modifierType;

        public float Value => _value;

        public int Order => (int)_modifierType;
        public object Source => _source;

        [SerializeField] private T _statType;
        [SerializeField] private StatModifierType _modifierType;

        [Space]
        [SerializeField] private float _value;

        private object _source;

        public StatModifier(T statType, StatModifierType modifierType, float value, object source = null)
        {
            _statType = statType;
            _modifierType = modifierType;

            _value = value;
            _source = source;
        }
    }
}
