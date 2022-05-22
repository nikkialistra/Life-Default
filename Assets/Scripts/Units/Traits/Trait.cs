using System.Collections.Generic;
using Units.Stats;
using UnityEngine;

namespace Units.Traits
{
    [CreateAssetMenu(fileName = "Unit Trait", menuName = "Trait/Unit Trait")]
    public class Trait : ScriptableObject
    {
        [SerializeField] private string _name;
        [TextArea]
        [SerializeField] private string _description;

        [SerializeField] private List<StatModifier> _statModifiers;

        public IReadOnlyCollection<StatModifier> StatModifiers => _statModifiers;
    }
}
