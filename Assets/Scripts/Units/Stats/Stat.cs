using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using UniRx;
using UnityEngine;

namespace Units.Stats
{
    [Serializable]
    public class Stat
    {
        [SerializeField] private float _baseValue;

        public float Value => ReactiveValue.Value;

        public IReadOnlyReactiveProperty<float> ReactiveValue;
        public ReadOnlyCollection<StatModifier> StatModifiers;
        
        private ReactiveProperty<bool> _isDirty = new();
        private readonly List<StatModifier> _statModifiers = new();

        public void Initialize()
        {
            StatModifiers = _statModifiers.AsReadOnly();

            ReactiveValue = _isDirty.ObserveEveryValueChanged(x => x.Value)
                .Where(x => x == true)
                .Select(_ =>
                {
                    _isDirty.Value = false;
                    return CalculateFinalValue();
                })
                .ToReactiveProperty();

            _isDirty.Value = true;
        }

        public virtual void AddModifier(StatModifier modifier)
        {
            _statModifiers.Add(modifier);
            _isDirty.Value = true;
        }

        public virtual bool RemoveModifier(StatModifier modifier)
        {
            if (_statModifiers.Remove(modifier))
            {
                _isDirty.Value = true;
                return true;
            }

            return false;
        }

        public virtual bool RemoveAllModifiersFromSource(object source)
        {
            var removedCount = _statModifiers.RemoveAll(modifier => modifier.Source == source);

            if (removedCount > 0)
            {
                _isDirty.Value = true;
                return true;
            }

            return false;
        }

        private float CalculateFinalValue()
        {
            var finalValue = _baseValue;
            var percentAddSum = 0f;

            _statModifiers.Sort(CompareModifierOrder);

            for (var i = 0; i < _statModifiers.Count; i++)
            {
                var modifier = _statModifiers[i];

                switch (modifier.Type)
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
            return (float)Math.Round(finalValue, 4);
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
            return i + 1 >= _statModifiers.Count || _statModifiers[i + 1].Type != StatModifierType.PercentAdd;
        }
    }
}
