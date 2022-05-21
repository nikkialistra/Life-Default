using System.Collections.Generic;
using Units.Stats;
using UnityEngine;

namespace Units.Traits
{
    public class Trait : ScriptableObject
    {
        [SerializeField] private string _name;
        [TextArea]
        [SerializeField] private string _description;

        [SerializeField] private List<StatModifier> _statModifiers;
    }
}
