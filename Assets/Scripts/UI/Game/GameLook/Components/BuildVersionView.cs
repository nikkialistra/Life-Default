using UnityEngine;
using UnityEngine.UIElements;

namespace UI.Game.GameLook.Components
{
    public class BuildVersionView : MonoBehaviour
    {
        private Label _label;

        private void Awake()
        {
            Tree = Resources.Load<VisualTreeAsset>("UI/Markup/GameLook/Components/BuildVersion").CloneTree();

            _label = Tree.Q<Label>("label");
        }

        public VisualElement Tree { get; private set; }

        private void Start()
        {
            _label.text = $"[Build: {Application.version}]";
        }
    }
}
