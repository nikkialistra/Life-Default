using Game.Units;
using UnityEngine;
using UnityEngine.UIElements;

namespace Kernel.UI.Game
{
    [RequireComponent(typeof(UIDocument))]
    public class InfoPanelView : MonoBehaviour
    {
        private VisualElement _tree;
        
        private Label _entityType;
        private Label _entityName;
        private ProgressBar _health;
        
        private UnitFacade _unit;

        private void Awake()
        {
            _tree = GetComponent<UIDocument>().rootVisualElement;

            _entityType = _tree.Q<Label>("entity_type");
            _entityName = _tree.Q<Label>("entity_name");
            _health = _tree.Q<ProgressBar>("health_progress_bar");
        }

        public void SetUnit(UnitFacade unitFacade)
        {
            _unit = unitFacade;
        }
    }
}