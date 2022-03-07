using System;
using Sirenix.OdinInspector;
using Units.Unit;
using Units.Unit.UnitTypes;
using UnityEngine;
using UnityEngine.UIElements;

namespace UI.Game.GameLook.Components
{
    [RequireComponent(typeof(InfoPanelView))]
    [RequireComponent(typeof(ChangeColorFractions))]
    public class ColonistInfoView : MonoBehaviour
    {
        private const string VisualTreePath = "UI/Markup/GameLook/Components/ColonistInfo";

        private UnitFacade _lastUnit;

        private InfoPanelView _parent;
        private TemplateContainer _tree;

        private Button _next;
        private Button _focus;
        
        private VisualElement _picture;


        private void Awake()
        {
            _parent = GetComponent<InfoPanelView>();

            _tree = Resources.Load<VisualTreeAsset>(VisualTreePath).CloneTree();

            _next = _tree.Q<Button>("next");
            _focus = _tree.Q<Button>("focus");

            _picture = _tree.Q<VisualElement>("picture");
        }

        private void OnDestroy()
        {
            UnsubscribeFromLastUnit();
        }

        public void ShowSelf()
        {
            _parent.InfoPanel.Add(_tree);
        }

        public void HideSelf()
        {
            if (_parent.InfoPanel.Contains(_tree))
            {
                _parent.InfoPanel.Remove(_tree);
            }
        }

        private void HidePanel()
        {
            _parent.HideSelf();
        }

        public void FillIn(UnitFacade unit)
        {
            FillInPreview(unit);
            FillInProperties(unit);
        }

        private void FillInPreview(UnitFacade unit)
        {
            // _image.style.backgroundImage = ;
        }

        private void FillInProperties(UnitFacade unit)
        {
            UnsubscribeFromLastUnit();
            _lastUnit = unit;

            // _nominationType.text = unit.UnitType.ToString();
            // _nominationName.text = unit.Name;

            ChangeHealth();

            SubscribeToUnit(unit);
        }

        private void SubscribeToUnit(UnitFacade unit)
        {
            unit.HealthChange += ChangeHealth;
            unit.Die += HidePanel;
        }

        private void UnsubscribeFromLastUnit()
        {
            if (_lastUnit != null)
            {
                _lastUnit.HealthChange -= ChangeHealth;
                _lastUnit.Die -= HidePanel;
            }
        }

        private void ChangeHealth()
        {
            // _health.value = (float)_lastUnit.Health / _lastUnit.MaxHealth;

            SetHealthColor();
        }

        private void SetHealthColor()
        {
            // var fraction = _health.value;
            // if (fraction > _changeColorFractions.Middle)
            // {
            //     _health.RemoveFromClassList("middle-health");
            //     _health.RemoveFromClassList("low-health");
            // }
            // else if (fraction > _changeColorFractions.Low)
            // {
            //     _health.AddToClassList("middle-health");
            //     _health.RemoveFromClassList("low-health");
            // }
            // else
            // {
            //     _health.RemoveFromClassList("middle-health");
            //     _health.AddToClassList("low-health");
            // }
        }
    }
}
