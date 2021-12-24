using System;
using System.Collections.Generic;
using Game.Units;
using UnityEngine;
using UnityEngine.UIElements;
using UnityEngine.UIElements.Experimental;

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

        private struct EventArgs
        {
            public VisualElement Sender;
            public UnitType UnitType;
        }

        private void Awake()
        {
            var args = new EventArgs
            {
                Sender = _travelerType,
                UnitType = UnitType.Traveler
            };
            
            _tree = GetComponent<UIDocument>().rootVisualElement;

            FillInTypes();
            FillInLabels();
        }

        private void OnEnable()
        {
            _travelerType.RegisterCallback<MouseDownEvent, EventArgs>(OnMouseDownEvent, new EventArgs { Sender = _travelerType, UnitType = UnitType.Traveler });
            _lumberjackType.RegisterCallback<MouseDownEvent, EventArgs>(OnMouseDownEvent, new EventArgs { Sender = _travelerType, UnitType = UnitType.Lumberjack });
            _masonType.RegisterCallback<MouseDownEvent, EventArgs>(OnMouseDownEvent, new EventArgs { Sender = _travelerType, UnitType = UnitType.Mason });
            _meleeType.RegisterCallback<MouseDownEvent, EventArgs>(OnMouseDownEvent, new EventArgs { Sender = _travelerType, UnitType = UnitType.Melee });
            _archerType.RegisterCallback<MouseDownEvent, EventArgs>(OnMouseDownEvent, new EventArgs { Sender = _travelerType, UnitType = UnitType.Archer });
        }

        private void OnDisable()
        {
            _travelerType.UnregisterCallback<MouseDownEvent, EventArgs>(OnMouseDownEvent);
            _lumberjackType.UnregisterCallback<MouseDownEvent, EventArgs>(OnMouseDownEvent);
            _masonType.UnregisterCallback<MouseDownEvent, EventArgs>(OnMouseDownEvent);
            _meleeType.UnregisterCallback<MouseDownEvent, EventArgs>(OnMouseDownEvent);
            _archerType.UnregisterCallback<MouseDownEvent, EventArgs>(OnMouseDownEvent);
        }

        public void ChangeUnitTypeCount(UnitType unitType, float count)
        {
            if (!_unitTypeLabels.ContainsKey(unitType))
            {
                throw new ArgumentException($"Dictionary doesn't contain key {unitType}");
            }

            var label = _unitTypeLabels[unitType];
            
            label.text = $"{count}";
            label.parent.style.opacity = count == 0 ? .5f : 1f;
        }

        private void OnMouseDownEvent(MouseDownEvent mouseDownEvent, EventArgs eventArgs)
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
