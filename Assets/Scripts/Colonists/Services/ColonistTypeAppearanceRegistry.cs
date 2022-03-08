using System;
using System.Collections.Generic;
using Colonists.Colonist.ColonistTypes;
using Sirenix.OdinInspector;
using UnityEngine;
using Random = UnityEngine.Random;

namespace Colonists.Services
{
    public class ColonistTypeAppearanceRegistry : MonoBehaviour
    {
        [ValidateInput("@_scouts.Count >= 1", "List should have at least one element.")]
        [SerializeField] private List<ColonistTypeAppearance> _scouts;
        [ValidateInput("@_lumberjacks.Count >= 1", "List should have at least one element.")]
        [SerializeField] private List<ColonistTypeAppearance> _lumberjacks;
        [ValidateInput("@_masons.Count >= 1", "List should have at least one element.")]
        [SerializeField] private List<ColonistTypeAppearance> _masons;
        [ValidateInput("@_melees.Count >= 1", "List should have at least one element.")]
        [SerializeField] private List<ColonistTypeAppearance> _melees;
        [ValidateInput("@_archers.Count >= 1", "List should have at least one element.")]
        [SerializeField] private List<ColonistTypeAppearance> _archers;

        public ColonistTypeAppearance GetForType(ColonistType colonistType)
        {
            return colonistType switch
            {
                ColonistType.Scout => GetFrom(_scouts),
                ColonistType.Lumberjack => GetFrom(_lumberjacks),
                ColonistType.Mason => GetFrom(_masons),
                ColonistType.Melee => GetFrom(_melees),
                ColonistType.Archer => GetFrom(_archers),
                _ => throw new ArgumentOutOfRangeException(nameof(colonistType), colonistType, null)
            };
        }

        private ColonistTypeAppearance GetFrom(List<ColonistTypeAppearance> colonistTypeAppearances)
        {
            return colonistTypeAppearances[Random.Range(0, colonistTypeAppearances.Count)];
        }
    }
}
