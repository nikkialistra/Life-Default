using UnityEngine;
using UnityEngine.UIElements;

namespace UI.Game.GameLook.Components
{
    public class CommandsView : MonoBehaviour
    {
        private const string VisualTreePath = "UI/Markup/GameLook/Components/Commands";

        private Button _move;
        private Button _stop;
        private Button _attack;
        private Button _hold;
        private Button _patrol;
        
        private InfoPanelView _parent;
        
        private TemplateContainer _tree;

        private void Awake()
        {
            _parent = GetComponent<InfoPanelView>();
            
            _tree = Resources.Load<VisualTreeAsset>(VisualTreePath).CloneTree();

            _move = _tree.Q<Button>("move");
            _stop = _tree.Q<Button>("stop");
            _attack = _tree.Q<Button>("attack");
            _hold = _tree.Q<Button>("hold");
            _patrol = _tree.Q<Button>("patrol");
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
    }
}
