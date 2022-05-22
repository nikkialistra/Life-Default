using System;
using UnityEngine;

namespace Units.Stats
{
    [Serializable]
    public class StatModifier
    {
        [SerializeField] private StatType _type;
        [SerializeField] private StatModifierType _modifierType;

        [Space]
        [SerializeField] private float _value;

        private object _source;
        
        public StatModifier(StatType type, StatModifierType modifierType, float value, object source = null)
        {
            _type = type;
            _modifierType = modifierType;

            _value = value;
            _source = source;
        }

        public StatType Type => _type;
        public StatModifierType ModifierType => _modifierType;

        public float Value => _value;

        public int Order => (int)_modifierType;
        public object Source => _source;
    }
}
