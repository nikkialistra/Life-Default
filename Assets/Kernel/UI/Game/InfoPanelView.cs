using System;
using Game.Units;
using UnityEngine;
using UnityEngine.UIElements;

namespace Kernel.UI.Game
{
    [RequireComponent(typeof(UIDocument))]
    public class InfoPanelView : MonoBehaviour
    {
        private VisualElement _tree;

        private VisualElement _infoPanel;
        
        private Label _entityType;
        private Label _entityName;
        private ProgressBar _health;

        private UnitFacade _unit;

        private void Awake()
        {
            _tree = GetComponent<UIDocument>().rootVisualElement;

            _infoPanel = _tree.Q<VisualElement>("info_panel");

            _entityType = _tree.Q<Label>("entity_type");
            _entityName = _tree.Q<Label>("entity_name");
            _health = _tree.Q<ProgressBar>("health_progress_bar");
        }

        private void Start()
        {
            _infoPanel.AddToClassList("not_displayed");
        }

        public void SetUnit(UnitFacade unitFacade)
        {
            _unit = unitFacade;

            FillIn();
        }

        private void FillIn()
        {
            _infoPanel.RemoveFromClassList("not_displayed");
            _entityType.text = _unit.UnitType.ToString();
            _entityName.text = _unit.Name;
            _health.value = _unit.Health;
        }
    }
}