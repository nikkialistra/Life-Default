using UnityEngine;
using UnityEngine.UIElements;

namespace UI.Game.GameLook.Components
{
    public class BuildVersionView : MonoBehaviour
    {
        [SerializeField] private VisualTreeAsset _asset;

        private Label _label;

        private void Awake()
        {
            Tree = _asset.CloneTree();

            _label = Tree.Q<Label>("label");
        }

        public VisualElement Tree { get; private set; }

        private void Start()
        {
            _label.text = $"[Build: {Application.version}]";
        }
    }
}
