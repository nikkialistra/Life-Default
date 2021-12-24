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
        public event Action<UnitType> LeftClick;
        public event Action<UnitType> RightClick;
        
        private VisualElement _tree;
        
        private readonly Dictionary<UnitType, Label> _unitTypeLabels = new();
        
        private VisualElement _travelerType;
        private VisualElement _lumberjackType;
        private VisualElement _masonType;
        private VisualElement _meleeType;
        private VisualElement _archerType;

        private void Awake()
        {
            _tree = GetComponent<UIDocument>().rootVisualElement;

            FillInTypes();
            FillInLabels();
        }

        private void OnEnable()
        {
            _travelerType.RegisterCallback<MouseDownEvent, UnitType>(OnMouseDownEvent, UnitType.Traveler);
            _lumberjackType.RegisterCallback<MouseDownEvent, UnitType>(OnMouseDownEvent, UnitType.Lumberjack);
            _masonType.RegisterCallback<MouseDownEvent, UnitType>(OnMouseDownEvent, UnitType.Mason);
            _meleeType.RegisterCallback<MouseDownEvent, UnitType>(OnMouseDownEvent, UnitType.Melee);
            _archerType.RegisterCallback<MouseDownEvent, UnitType>(OnMouseDownEvent, UnitType.Archer);
        }

        private void OnDisable()
        {
            _travelerType.UnregisterCallback<MouseDownEvent, UnitType>(OnMouseDownEvent);
            _lumberjackType.UnregisterCallback<MouseDownEvent, UnitType>(OnMouseDownEvent);
            _masonType.UnregisterCallback<MouseDownEvent, UnitType>(OnMouseDownEvent);
            _meleeType.UnregisterCallback<MouseDownEvent, UnitType>(OnMouseDownEvent);
            _archerType.UnregisterCallback<MouseDownEvent, UnitType>(OnMouseDownEvent);
        }

        public void ChangeUnitTypeCount(UnitType unitType, float count)
        {
            if (!_unitTypeLabels.ContainsKey(unitType))
            {
                throw new ArgumentException($"Dictionary doesn't contain key {unitType}");
            }
            
            _unitTypeLabels[unitType].text = $"{count}";
        }

        private void OnMouseDownEvent(MouseDownEvent mouseDownEvent, UnitType unitType)
        {
            if (mouseDownEvent.button == 0)
            {
                LeftClick?.Invoke(unitType);
            }

            if (mouseDownEvent.button == 1)
            {
                RightClick?.Invoke(unitType);
            }
        }

        private void FillInTypes()
        {
            _travelerType = _tree.Q<VisualElement>("traveler_type");
            _lumberjackType = _tree.Q<VisualElement>("lumberjack_type");
            _masonType = _tree.Q<VisualElement>("mason_type");
            _meleeType = _tree.Q<VisualElement>("melee_type");
            _archerType = _tree.Q<VisualElement>("archer_type");
        }

        private void FillInLabels()
        {
            _unitTypeLabels.Add(UnitType.Traveler, _tree.Q<Label>("traveler_type_count"));
            _unitTypeLabels.Add(UnitType.Lumberjack, _tree.Q<Label>("lumberjack_type_count"));
            _unitTypeLabels.Add(UnitType.Mason, _tree.Q<Label>("mason_type_count"));
            _unitTypeLabels.Add(UnitType.Melee, _tree.Q<Label>("melee_type_count"));
            _unitTypeLabels.Add(UnitType.Archer, _tree.Q<Label>("archer_type_count"));
        }
    }
}
