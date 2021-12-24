using System;
using System.Collections.Generic;
using Game.Units;
using UnityEngine;
using UnityEngine.UIElements;

namespace Kernel.UI.GameViews
{
    [RequireComponent(typeof(UIDocument))]
    public class UnitTypesView : MonoBehaviour
    {
        private VisualElement _tree;
        
        private readonly Dictionary<UnitType, Label> _unitTypeLabels = new();

        private void Awake()
        {
            _tree = GetComponent<UIDocument>().rootVisualElement;
            
            _unitTypeLabels.Add(UnitType.Traveler, _tree.Q<Label>("traveler_type_count"));
            _unitTypeLabels.Add(UnitType.Lumberjack, _tree.Q<Label>("lumberjack_type_count"));
            _unitTypeLabels.Add(UnitType.Mason, _tree.Q<Label>("mason_type_count"));
            _unitTypeLabels.Add(UnitType.Melee, _tree.Q<Label>("melee_type_count"));
            _unitTypeLabels.Add(UnitType.Archer, _tree.Q<Label>("archer_type_count"));
        }

        public void ChangeUnitTypeCount(UnitType unitType, float count)
        {
            if (!_unitTypeLabels.ContainsKey(unitType))
            {
                throw new ArgumentException($"Dictionary doesn't contain key {unitType}");
            }
            
            _unitTypeLabels[unitType].text = $"{count}";
        }
    }
}
