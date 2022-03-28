using UnityEngine;
using UnityEngine.UIElements;

namespace UI.Game.GameLook.Components
{
    [RequireComponent(typeof(InfoPanelView))]
    [RequireComponent(typeof(CommandsView))]
    public class ColonistsInfoView : MonoBehaviour
    {
        private const string VisualTreePath = "UI/Markup/GameLook/Components/ColonistsInfo";

        private InfoPanelView _parent;
        private TemplateContainer _tree;
        
        private Label _count;

        private VisualElement _commands;

        private bool _shown;
        
        private CommandsView _commandsView;

        private void Awake()
        {
            _parent = GetComponent<InfoPanelView>();

            _tree = Resources.Load<VisualTreeAsset>(VisualTreePath).CloneTree();
            _tree.pickingMode = PickingMode.Ignore;
            
            _commandsView = GetComponent<CommandsView>();

            _count = _tree.Q<Label>("count");
            
            _commands = _tree.Q<VisualElement>("commands");
        }
        
        public void ShowSelf()
        {
            if (_shown)
            {
                return;
            }

            _parent.InfoPanel.Add(_tree);
            _commandsView.BindSelf(_commands);
            _shown = true;
        }

        public void HideSelf()
        {
            if (!_shown)
            {
                return;
            }
            
            _parent.InfoPanel.Remove(_tree);
            _commandsView.UnbindSelf();
            _shown = false;
        }

        public void SetCount(int count)
        {
            _count.text = $"Colonists ({count})";
        }
    }
}
