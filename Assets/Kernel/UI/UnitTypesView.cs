using System;
using System.Collections.Generic;
using Game.Units;
using UnityEngine;
using UnityEngine.UIElements;

namespace Kernel.UI
{
    [RequireComponent(typeof(UIDocument))]
    public class UnitTypesView : MonoBehaviour
    {
        private VisualElement _rootVisualElement;
        
        private Label _travelerType;

        private readonly Dictionary<UnitType, Label> _unitTypeLabels = new();

        private void Awake()
        {
            _rootVisualElement = GetComponent<UIDocument>().rootVisualElement;
            
            _unitTypeLabels.Add(UnitType.Traveler, _rootVisualElement.Q<Label>("traveler_type_count"));
            _unitTypeLabels.Add(UnitType.Lumberjack, _rootVisualElement.Q<Label>("lumberjack_type_count"));
            _unitTypeLabels.Add(UnitType.Mason, _rootVisualElement.Q<Label>("mason_type_count"));
            _unitTypeLabels.Add(UnitType.Melee, _rootVisualElement.Q<Label>("melee_type_count"));
            _unitTypeLabels.Add(UnitType.Archer, _rootVisualElement.Q<Label>("archer_type_count"));
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
