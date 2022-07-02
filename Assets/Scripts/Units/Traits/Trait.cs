using System.Collections.Generic;
using Sirenix.OdinInspector;
using Units.Stats;
using UnityEngine;

namespace Units.Traits
{
    [CreateAssetMenu(fileName = "Unit Trait", menuName = "Trait/Unit Trait")]
    public class Trait : ScriptableObject
    {
        public Sprite Icon => _icon;

        public string Name => _name;
        public string Description => _description;

        public IReadOnlyCollection<StatModifier<UnitStat>> UnitStatModifiers => _unitStatModifiers;
        public IReadOnlyCollection<StatModifier<ColonistStat>> ColonistStatModifiers => _colonistStatModifiers;

        [HorizontalGroup("Split", 100)]
        [VerticalGroup("Split/Left")]
        [HideLabel]
        [PreviewField(80, ObjectFieldAlignment.Left)]
        [SerializeField] private Sprite _icon;

        [VerticalGroup("Split/Right")]
        [SerializeField] private string _name;
        [VerticalGroup("Split/Right")]
        [TextArea]
        [SerializeField] private string _description;

        [Space]
        [SerializeField] private List<StatModifier<UnitStat>> _unitStatModifiers;
        [SerializeField] private List<StatModifier<ColonistStat>> _colonistStatModifiers;
    }
}
