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
        
        private Label _type;
        private Label _name;
        private ProgressBar _health;

        private UnitFacade _unit;

        private void Awake()
        {
            _tree = GetComponent<UIDocument>().rootVisualElement;

            _infoPanel = _tree.Q<VisualElement>("info-panel");

            _type = _tree.Q<Label>("nomination__type");
            _name = _tree.Q<Label>("nomination__name");
            _health = _tree.Q<ProgressBar>("health__progress-bar");
        }

        private void Start()
        {
            _infoPanel.AddToClassList("not-displayed");
        }

        public void SetUnit(UnitFacade unitFacade)
        {
            _unit = unitFacade;

            FillIn();
        }

        private void FillIn()
        {
            _infoPanel.RemoveFromClassList("not-displayed");
            _type.text = _unit.UnitType.ToString();
            _name.text = _unit.Name;
            _health.value = _unit.Health;
        }
    }
}