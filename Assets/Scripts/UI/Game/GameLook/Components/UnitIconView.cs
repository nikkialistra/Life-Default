using System;
using System.Collections.Generic;
using Units.Unit;
using Units.Unit.UnitTypes;
using UnityEngine;
using UnityEngine.UIElements;

namespace UI.Game.GameLook.Components
{
    public class UnitIconView : IDisposable
    {
        private const string VisualTreePath = "UI/Markup/GameLook/Components/UnitIcon";
        
        private UnitFacade _unit;

        private readonly UnitsInfoView _parent;
        private readonly Dictionary<UnitType, Texture2D> _previews;

        private readonly TemplateContainer _tree;
        private readonly VisualElement _root;
        private readonly VisualElement _image;
        private readonly ProgressBar _health;
        private readonly ChangeColorFractions _changeColorFractions;

        public UnitIconView(UnitsInfoView parent, Dictionary<UnitType, Texture2D> previews,
            ChangeColorFractions changeColorFractions)
        {
            _parent = parent;

            if (previews.Count != 5)
            {
                throw new ArgumentException("Preview dictionary should have 5 elements for all 5 unit types");
            }

            _previews = previews;
            _changeColorFractions = changeColorFractions;

            _tree = Resources.Load<VisualTreeAsset>(VisualTreePath).CloneTree();

            _root = _tree.Q<VisualElement>("unit-icon");
            _image = _tree.Q<VisualElement>("image");
            _health = _tree.Q<ProgressBar>("health__progress-bar");
        }

        public event Action<UnitFacade> Click;
        public event Action Remove;

        public void Bind(UnitFacade unit)
        {
            _unit = unit;

            ShowSelf();
            RegisterToClicks();
            SetIconImage();
            SetHealth();
            SubscribeToEvents();
        }

        private void ShowSelf()
        {
            _parent.IconContainer.Add(_tree);
        }

        private void HideSelf()
        {
            Unbind();
            _parent.IconContainer.Remove(_tree);
            Remove?.Invoke();
        }

        private void RegisterToClicks()
        {
            _root.UnregisterCallback<MouseDownEvent, UnitFacade>(IconOnMouseDownEvent);
            _root.RegisterCallback<MouseDownEvent, UnitFacade>(IconOnMouseDownEvent, _unit);
        }

        private void IconOnMouseDownEvent(MouseDownEvent mouseDownEvent, UnitFacade unit)
        {
            Click?.Invoke(unit);
        }

        private void SetIconImage()
        {
            _image.style.backgroundImage = _unit.UnitType switch
            {
                UnitType.Scout => new StyleBackground(_previews[UnitType.Scout]),
                UnitType.Lumberjack => new StyleBackground(_previews[UnitType.Lumberjack]),
                UnitType.Mason => new StyleBackground(_previews[UnitType.Mason]),
                UnitType.Melee => new StyleBackground(_previews[UnitType.Melee]),
                UnitType.Archer => new StyleBackground(_previews[UnitType.Archer]),
                _ => throw new ArgumentOutOfRangeException()
            };
        }

        private void SetHealth()
        {
            _health.value = (float)_unit.Health / _unit.MaxHealth;
            SetHealthColor();
        }

        private void SetHealthColor()
        {
            var fraction = _health.value;
            if (fraction > _changeColorFractions.Middle)
            {
                _health.RemoveFromClassList("middle-health");
                _health.RemoveFromClassList("low-health");
            }
            else if (fraction > _changeColorFractions.Low)
            {
                _health.AddToClassList("middle-health");
                _health.RemoveFromClassList("low-health");
            }
            else
            {
                _health.RemoveFromClassList("middle-health");
                _health.AddToClassList("low-health");
            }
        }

        private void SubscribeToEvents()
        {
            _unit.HealthChange += SetHealth;
            _unit.Die += HideSelf;
        }

        public void Dispose()
        {
            if (_unit == null)
            {
                return;
            }

            _root.UnregisterCallback<MouseDownEvent, UnitFacade>(IconOnMouseDownEvent);
            _unit.HealthChange -= SetHealth;
            _unit.Die -= HideSelf;
        }

        public void Unbind()
        {
            if (_unit == null)
            {
                return;
            }

            _unit.HealthChange -= SetHealth;
            _unit.Die -= HideSelf;
            _unit = null;
        }
    }
}
