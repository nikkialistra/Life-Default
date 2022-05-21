using System;
using UnityEngine;

namespace Units.Stats
{
    [Serializable]
    public class StatModifier
    {
        public StatType StatType;
        public StatModifierType Type;

        [Space]
        public float Value;

        public int Order { get; private set; }
        public object Source { get; private set; }
    }
}
