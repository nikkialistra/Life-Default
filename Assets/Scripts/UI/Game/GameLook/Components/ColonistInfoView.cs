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

        private Label _name;
        
        private Button _next;
        private Button _focus;

        private VisualElement _picture;

        private ProgressBar _healthProgress;
        private Label _healthValue;
        private VisualElement _healthArrow;


        private void Awake()
        {
            _parent = GetComponent<InfoPanelView>();

            _tree = Resources.Load<VisualTreeAsset>(VisualTreePath).CloneTree();

            _name = _tree.Q<Label>("name");

            _next = _tree.Q<Button>("next");
            _focus = _tree.Q<Button>("focus");

            _picture = _tree.Q<VisualElement>("picture");

            _healthProgress = _tree.Q<ProgressBar>("health-progress");
            _healthValue = _tree.Q<Label>("health-value");
            _healthArrow = _tree.Q<VisualElement>("health-arrow");
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
            
            _name.text = unit.Name;

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
            _healthProgress.value = (float)_lastUnit.Health / _lastUnit.MaxHealth;
            _healthValue.text = $"{_healthProgress.value * 100}%";
        }
    }
}
