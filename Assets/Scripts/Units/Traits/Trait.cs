using System.Collections.Generic;
using Sirenix.OdinInspector;
using Units.Stats;
using UnityEngine;

namespace Units.Traits
{
    [CreateAssetMenu(fileName = "Unit Trait", menuName = "Trait/Unit Trait")]
    public class Trait : ScriptableObject
    {
        [HorizontalGroup("Split", 100)]
        [VerticalGroup("Split/Left")]
        [HideLabel]
        [SerializeField] private Sprite _icon;

        [VerticalGroup("Split/Right")]
        [SerializeField] private string _name;
        [VerticalGroup("Split/Right")]
        [TextArea]
        [SerializeField] private string _description;

        [Space]
        [SerializeField] private List<StatModifier> _statModifiers;

        public Sprite Icon => _icon;
        
        public string Name => _name;
        public string Description => _description;
        
        public IReadOnlyCollection<StatModifier> StatModifiers => _statModifiers;
    }
}
