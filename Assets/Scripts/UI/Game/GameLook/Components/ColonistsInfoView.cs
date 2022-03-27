using UnityEngine;
using UnityEngine.UIElements;

namespace UI.Game.GameLook.Components
{
    [RequireComponent(typeof(InfoPanelView))]
    public class ColonistsInfoView : MonoBehaviour
    {
        private const string VisualTreePath = "UI/Markup/GameLook/Components/ColonistInfo";

        private InfoPanelView _parent;
        private TemplateContainer _tree;
        
        private Label _count;
        
        private bool _shown;

        private void Awake()
        {
            _parent = GetComponent<InfoPanelView>();

            _tree = Resources.Load<VisualTreeAsset>(VisualTreePath).CloneTree();
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
            _count.text = $"Colonists ({count})";
        }
    }
}
