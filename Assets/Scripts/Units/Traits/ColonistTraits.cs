using System;
using System.Collections.Generic;
using Colonists;
using UnityEngine;

namespace Units.Traits
{
    [RequireComponent(typeof(ColonistStats))]
    public class ColonistTraits : MonoBehaviour
    {
        [SerializeField] private List<Trait> _traits = new();

        private ColonistStats _colonistStats;

        public IReadOnlyList<Trait> Traits;

        private void Awake()
        {
            _colonistStats = GetComponent<ColonistStats>();

            Traits = _traits.AsReadOnly();
        }

        private void Start()
        {
            ApplyTraits();
        }

        public void AddTrait(Trait trait)
        {
            if (_traits.Contains(trait))
            {
                throw new InvalidOperationException("Cannot add trait twice");
            }
            
            _traits.Add(trait);
            ApplyTrait(trait);
        }

        public void RemoveTrait(Trait trait)
        {
            if (!_traits.Contains(trait))
            {
                throw new InvalidOperationException("Cannot remove not contained trait");
            }

            _traits.Remove(trait);
            DiscardTrait(trait);
        }

        private void ApplyTraits()
        {
            foreach (var trait in _traits)
            {
                ApplyTrait(trait);
            }
        }

        private void ApplyTrait(Trait trait)
        {
            foreach (var statModifier in trait.ColonistStatModifiers)
            {
                _colonistStats.AddStatModifier(statModifier);
            }
        }
        
        private void DiscardTrait(Trait trait)
        {
            foreach (var statModifier in trait.ColonistStatModifiers)
            {
                _colonistStats.RemoveStatModifier(statModifier);
            }
        }
    }
}
