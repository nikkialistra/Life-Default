using System;
using System.Collections.Generic;
using Units.Unit.UnitTypes;
using UnityEngine;
using UnityEngine.UIElements;

namespace UI.Game.GameLook.Components
{
    public class UnitTypesView : MonoBehaviour
    {
        private const string VisualTreePath = "UI/Markup/GameLook/Components/UnitTypes";
        
        private readonly Dictionary<UnitType, Label> _unitTypeLabels = new();

        private VisualElement _scoutType;
        private VisualElement _lumberjackType;
        private VisualElement _masonType;
        private VisualElement _meleeType;
        private VisualElement _archerType;

        private void Awake()
        {
            Tree = Resources.Load<VisualTreeAsset>(VisualTreePath).CloneTree();

            FillInTypes();
            FillInLabels();
        }

        public event Action<UnitType> LeftClick;
        public event Action<UnitType> RightClick;

        public VisualElement Tree { get; private set; }

        private void OnEnable()
        {
            _scoutType.RegisterCallback<MouseDownEvent, UnitType>(TypeOnMouseDownEvent, UnitType.Scout);
            _lumberjackType.RegisterCallback<MouseDownEvent, UnitType>(TypeOnMouseDownEvent, UnitType.Lumberjack);
            _masonType.RegisterCallback<MouseDownEvent, UnitType>(TypeOnMouseDownEvent, UnitType.Mason);
            _meleeType.RegisterCallback<MouseDownEvent, UnitType>(TypeOnMouseDownEvent, UnitType.Melee);
            _archerType.RegisterCallback<MouseDownEvent, UnitType>(TypeOnMouseDownEvent, UnitType.Archer);
        }

        private void OnDisable()
        {
            _scoutType.UnregisterCallback<MouseDownEvent, UnitType>(TypeOnMouseDownEvent);
            _lumberjackType.UnregisterCallback<MouseDownEvent, UnitType>(TypeOnMouseDownEvent);
            _masonType.UnregisterCallback<MouseDownEvent, UnitType>(TypeOnMouseDownEvent);
            _meleeType.UnregisterCallback<MouseDownEvent, UnitType>(TypeOnMouseDownEvent);
            _archerType.UnregisterCallback<MouseDownEvent, UnitType>(TypeOnMouseDownEvent);
        }

        public void UpdateUnitTypeCount(UnitType unitType, int value)
        {
            CheckUnitTypeExistence(unitType);

            var label = _unitTypeLabels[unitType];
            label.text = $"{value}";

            UpdateUnitTypeStyles(value, label);
        }

        private void CheckUnitTypeExistence(UnitType unitType)
        {
            if (!_unitTypeLabels.ContainsKey(unitType))
            {
                throw new ArgumentException($"Dictionary doesn't contain key {unitType}");
            }
        }

        private static void UpdateUnitTypeStyles(float count, Label label)
        {
            if (count == 0)
            {
                label.parent.AddToClassList("missing");
            }
            else
            {
                label.parent.RemoveFromClassList("missing");
            }
        }

        private void TypeOnMouseDownEvent(MouseDownEvent mouseDownEvent, UnitType unitType)
        {
            switch (mouseDownEvent.button)
            {
                case 0:
                    LeftClick?.Invoke(unitType);
                    break;
                case 1:
                    RightClick?.Invoke(unitType);
                    break;
            }
        }

        private void FillInTypes()
        {
            _scoutType = Tree.Q<VisualElement>("scout-type");
            _lumberjackType = Tree.Q<VisualElement>("lumberjack-type");
            _masonType = Tree.Q<VisualElement>("mason-type");
            _meleeType = Tree.Q<VisualElement>("melee-type");
            _archerType = Tree.Q<VisualElement>("archer-type");
        }

        private void FillInLabels()
        {
            _unitTypeLabels.Add(UnitType.Scout, Tree.Q<Label>("scout-type__count"));
            _unitTypeLabels.Add(UnitType.Lumberjack, Tree.Q<Label>("lumberjack-type__count"));
            _unitTypeLabels.Add(UnitType.Mason, Tree.Q<Label>("mason-type__count"));
            _unitTypeLabels.Add(UnitType.Melee, Tree.Q<Label>("melee-type__count"));
            _unitTypeLabels.Add(UnitType.Archer, Tree.Q<Label>("archer-type__count"));
        }
    }
}
