using System;
using System.Collections.Generic;
using Units.Unit.UnitType;
using UnityEngine;
using UnityEngine.UIElements;

namespace UI.Game
{
    [RequireComponent(typeof(UIDocument))]
    public class UnitTypesView : MonoBehaviour
    {
        private VisualElement _tree;

        private readonly Dictionary<UnitType, Label> _unitTypeLabels = new();

        private VisualElement _scoutType;
        private VisualElement _lumberjackType;
        private VisualElement _masonType;
        private VisualElement _meleeType;
        private VisualElement _archerType;

        private struct EventArgs
        {
            public VisualElement Sender;
            public UnitType UnitType;
        }

        private void Awake()
        {
            _tree = GetComponent<UIDocument>().rootVisualElement;

            FillInTypes();
            FillInLabels();
        }

        public event Action<UnitType> LeftClick;
        public event Action<UnitType> RightClick;

        private void OnEnable()
        {
            _scoutType.RegisterCallback<MouseDownEvent, EventArgs>(TypeOnMouseDownEvent,
                new EventArgs { Sender = _scoutType, UnitType = UnitType.Scout });
            _lumberjackType.RegisterCallback<MouseDownEvent, EventArgs>(TypeOnMouseDownEvent,
                new EventArgs { Sender = _lumberjackType, UnitType = UnitType.Lumberjack });
            _masonType.RegisterCallback<MouseDownEvent, EventArgs>(TypeOnMouseDownEvent,
                new EventArgs { Sender = _masonType, UnitType = UnitType.Mason });
            _meleeType.RegisterCallback<MouseDownEvent, EventArgs>(TypeOnMouseDownEvent,
                new EventArgs { Sender = _meleeType, UnitType = UnitType.Melee });
            _archerType.RegisterCallback<MouseDownEvent, EventArgs>(TypeOnMouseDownEvent,
                new EventArgs { Sender = _archerType, UnitType = UnitType.Archer });
        }

        private void OnDisable()
        {
            _scoutType.UnregisterCallback<MouseDownEvent, EventArgs>(TypeOnMouseDownEvent);
            _lumberjackType.UnregisterCallback<MouseDownEvent, EventArgs>(TypeOnMouseDownEvent);
            _masonType.UnregisterCallback<MouseDownEvent, EventArgs>(TypeOnMouseDownEvent);
            _meleeType.UnregisterCallback<MouseDownEvent, EventArgs>(TypeOnMouseDownEvent);
            _archerType.UnregisterCallback<MouseDownEvent, EventArgs>(TypeOnMouseDownEvent);
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

        private void TypeOnMouseDownEvent(MouseDownEvent mouseDownEvent, EventArgs eventArgs)
        {
            if (mouseDownEvent.button == 0)
            {
                LeftClick?.Invoke(eventArgs.UnitType);
            }

            if (mouseDownEvent.button == 1)
            {
                RightClick?.Invoke(eventArgs.UnitType);
            }
        }

        private void FillInTypes()
        {
            _scoutType = _tree.Q<VisualElement>("scout-type");
            _lumberjackType = _tree.Q<VisualElement>("lumberjack-type");
            _masonType = _tree.Q<VisualElement>("mason-type");
            _meleeType = _tree.Q<VisualElement>("melee-type");
            _archerType = _tree.Q<VisualElement>("archer-type");
        }

        private void FillInLabels()
        {
            _unitTypeLabels.Add(UnitType.Scout, _tree.Q<Label>("scout-type__count"));
            _unitTypeLabels.Add(UnitType.Lumberjack, _tree.Q<Label>("lumberjack-type__count"));
            _unitTypeLabels.Add(UnitType.Mason, _tree.Q<Label>("mason-type__count"));
            _unitTypeLabels.Add(UnitType.Melee, _tree.Q<Label>("melee-type__count"));
            _unitTypeLabels.Add(UnitType.Archer, _tree.Q<Label>("archer-type__count"));
        }
    }
}
