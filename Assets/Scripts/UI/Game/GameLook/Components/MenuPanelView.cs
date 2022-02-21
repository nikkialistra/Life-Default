using System;
using UnityEngine;
using UnityEngine.UIElements;

namespace UI.Game.GameLook.Components
{
    public class MenuPanelView : MonoBehaviour
    {
        private Button _menu;

        private void Awake()
        {
            Tree = Resources.Load<VisualTreeAsset>("UI/Markup/GameLook/Components/MenuPanel").CloneTree();

            _menu = Tree.Q<Button>("menu");
        }

        public VisualElement Tree { get; private set; }

        public event Action Click;

        private void OnEnable()
        {
            _menu.clicked += Menu;
        }

        private void OnDisable()
        {
            _menu.clicked -= Menu;
        }

        private void Menu()
        {
            Click?.Invoke();
        }
    }
}
