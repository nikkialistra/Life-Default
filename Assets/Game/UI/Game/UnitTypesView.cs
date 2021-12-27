using System;
using System.Collections.Generic;
using Game.Units.Unit;
using UnityEngine;
using UnityEngine.UIElements;

namespace Game.UI.Game
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

        private void OnEnable()
        {
            _travelerType.RegisterCallback<MouseDownEvent, EventArgs>(TypeOnMouseDownEvent, new EventArgs { Sender = _travelerType, UnitType = UnitType.Traveler });
            _lumberjackType.RegisterCallback<MouseDownEvent, EventArgs>(TypeOnMouseDownEvent, new EventArgs { Sender = _travelerType, UnitType = UnitType.Lumberjack });
            _masonType.RegisterCallback<MouseDownEvent, EventArgs>(TypeOnMouseDownEvent, new EventArgs { Sender = _travelerType, UnitType = UnitType.Mason });
            _meleeType.RegisterCallback<MouseDownEvent, EventArgs>(TypeOnMouseDownEvent, new EventArgs { Sender = _travelerType, UnitType = UnitType.Melee });
            _archerType.RegisterCallback<MouseDownEvent, EventArgs>(TypeOnMouseDownEvent, new EventArgs { Sender = _travelerType, UnitType = UnitType.Archer });
        }

        private void OnDisable()
        {
            _travelerType.UnregisterCallback<MouseDownEvent, EventArgs>(TypeOnMouseDownEvent);
            _lumberjackType.UnregisterCallback<MouseDownEvent, EventArgs>(TypeOnMouseDownEvent);
            _masonType.UnregisterCallback<MouseDownEvent, EventArgs>(TypeOnMouseDownEvent);
            _meleeType.UnregisterCallback<MouseDownEvent, EventArgs>(TypeOnMouseDownEvent);
            _archerType.UnregisterCallback<MouseDownEvent, EventArgs>(TypeOnMouseDownEvent);
        }

        public void ChangeUnitTypeCount(UnitType unitType, float count)
        {
            if (!_unitTypeLabels.ContainsKey(unitType))
            {
                throw new ArgumentException($"Dictionary doesn't contain key {unitType}");
            }

            var label = _unitTypeLabels[unitType];
            
            label.text = $"{count}";
            
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
            _travelerType = _tree.Q<VisualElement>("traveler-type");
            _lumberjackType = _tree.Q<VisualElement>("lumberjack-type");
            _masonType = _tree.Q<VisualElement>("mason-type");
            _meleeType = _tree.Q<VisualElement>("melee-type");
            _archerType = _tree.Q<VisualElement>("archer-type");
        }

        private void FillInLabels()
        {
            _unitTypeLabels.Add(UnitType.Traveler, _tree.Q<Label>("traveler-type__count"));
            _unitTypeLabels.Add(UnitType.Lumberjack, _tree.Q<Label>("lumberjack-type__count"));
            _unitTypeLabels.Add(UnitType.Mason, _tree.Q<Label>("mason-type__count"));
            _unitTypeLabels.Add(UnitType.Melee, _tree.Q<Label>("melee-type__count"));
            _unitTypeLabels.Add(UnitType.Archer, _tree.Q<Label>("archer-type__count"));
        }
    }
}