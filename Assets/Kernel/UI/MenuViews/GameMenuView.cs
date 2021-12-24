using UnityEngine;
using UnityEngine.UIElements;

namespace Kernel.UI.MenuViews
{
    public class GameMenuView : MonoBehaviour, IMenuView
    {
        private TemplateContainer _tree;
        
        private Button _resume;
        private Button _exitGame;

        private void Awake()
        {
            var template = Resources.Load<VisualTreeAsset>("UI/Markup/GameMenu");
            _tree = template.CloneTree();

            _resume = _tree.Q<Button>("resume");
            _exitGame = _tree.Q<Button>("exit_game");
        }

        public void ShowSelf()
        {
            throw new System.NotImplementedException();
        }

        public void HideSelf()
        {
            throw new System.NotImplementedException();
        }
    }
}