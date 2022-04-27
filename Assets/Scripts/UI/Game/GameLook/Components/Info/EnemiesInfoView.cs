using UnityEngine;
using UnityEngine.UIElements;

namespace UI.Game.GameLook.Components.Info
{
    [RequireComponent(typeof(InfoPanelView))]
    [RequireComponent(typeof(CommandsView))]
    public class EnemiesInfoView : MonoBehaviour
    {
        [SerializeField] private VisualTreeAsset _asset;

        private InfoPanelView _parent;
        private TemplateContainer _tree;
        
        private Label _count;

        private VisualElement _commands;

        private bool _shown;

        private void Awake()
        {
            _parent = GetComponent<InfoPanelView>();

            _tree = _asset.CloneTree();
            _tree.pickingMode = PickingMode.Ignore;

            _count = _tree.Q<Label>("count");
        }
        
        public void ShowSelf()
        {
            if (_shown)
            {
                return;
            }

            _parent.InfoPanel.Add(_tree);
            _shown = true;
        }

        public void HideSelf()
        {
            if (!_shown)
            {
                return;
            }
            
            _parent.InfoPanel.Remove(_tree);
            _shown = false;
        }

        public void SetCount(int count)
        {
            _count.text = $"Enemies ({count})";
        }
    }
}
