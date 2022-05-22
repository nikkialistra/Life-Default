using System;
using System.Collections.Generic;
using UnityEngine;

namespace Units.Stats
{
    [Serializable]
    public class Stat
    {
        [SerializeField] private float _baseValue;
        
        public float Value { get; private set; }
        
        public IReadOnlyList<StatModifier> StatModifiers;
        
        private readonly List<StatModifier> _statModifiers = new();

        public void Initialize()
        {
            StatModifiers = _statModifiers.AsReadOnly();

            RecalculateValue();
        }
        
        public event Action<float> ValueChange;

        public virtual void AddModifier(StatModifier modifier)
        {
            _statModifiers.Add(modifier);
            RecalculateValue();
        }

        public virtual bool RemoveModifier(StatModifier modifier)
        {
            if (_statModifiers.Remove(modifier))
            {
                RecalculateValue();
                return true;
            }

            return false;
        }

        public virtual bool RemoveAllModifiersFromSource(object source)
        {
            var removedCount = _statModifiers.RemoveAll(modifier => modifier.Source == source);

            if (removedCount > 0)
            {
                RecalculateValue();
                return true;
            }

            return false;
        }

        private void RecalculateValue()
        {
            var finalValue = _baseValue;
            var percentAddSum = 0f;

            _statModifiers.Sort(CompareModifierOrder);

            for (var i = 0; i < _statModifiers.Count; i++)
            {
                var modifier = _statModifiers[i];

                switch (modifier.ModifierType)
                {
                    case StatModifierType.EarlyFlat:
                        finalValue += modifier.Value;
                        break;
                    case StatModifierType.PercentAdd:
                    {
                        percentAddSum += modifier.Value;

                        if (IsPercentAddModifiersEnd(i))
                        {
                            finalValue *= 1 + percentAddSum;
                        }
                        break;
                    }
                    case StatModifierType.PercentMultiply:
                        finalValue *= 1 + modifier.Value;
                        break;
                    case StatModifierType.LateFlat:
                        finalValue += modifier.Value;
                        break;
                }
            }

            // Workaround for float calculation errors, like displaying 12.00001 instead of 12
            Value = (float)Math.Round(finalValue, 4);
            
            ValueChange?.Invoke(Value);
        }

        private int CompareModifierOrder(StatModifier a, StatModifier b)
        {
            if (a.Order < b.Order)
            {
                return -1;
            }

            if (a.Order > b.Order)
            {
                return 1;
            }

            return 0;
        }

        private bool IsPercentAddModifiersEnd(int i)
        {
            return i + 1 >= _statModifiers.Count || _statModifiers[i + 1].ModifierType != StatModifierType.PercentAdd;
        }
    }
}
