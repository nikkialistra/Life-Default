using System;
using System.Collections.Generic;
using Units.Stats;
using UnityEngine;

namespace Units.Traits
{
    [RequireComponent(typeof(UnitStats))]
    public class UnitTraits : MonoBehaviour
    {
        public IReadOnlyList<Trait> Traits { get; private set; }

        [SerializeField] private List<Trait> _traits = new();

        private UnitStats _unitStats;

        private void Awake()
        {
            _unitStats = GetComponent<UnitStats>();

            Traits = _traits.AsReadOnly();
        }

        private void Start()
        {
            ApplyTraits();
        }

        public void AddTrait(Trait trait)
        {
            if (_traits.Contains(trait)) throw new InvalidOperationException("Cannot add trait twice");

            _traits.Add(trait);
            ApplyTrait(trait);
        }

        public void RemoveTrait(Trait trait)
        {
            if (!_traits.Contains(trait)) throw new InvalidOperationException("Cannot remove not contained trait");

            _traits.Remove(trait);
            DiscardTrait(trait);
        }

        private void ApplyTraits()
        {
            foreach (var trait in _traits)
                ApplyTrait(trait);
        }

        private void ApplyTrait(Trait trait)
        {
            foreach (var statModifier in trait.UnitStatModifiers)
                _unitStats.AddStatModifier(statModifier);
        }

        private void DiscardTrait(Trait trait)
        {
            foreach (var statModifier in trait.UnitStatModifiers)
                _unitStats.RemoveStatModifier(statModifier);
        }
    }
}
