using UnityEngine;
using UnityEngine.UIElements;

namespace UI.Game
{
    public class BuildVersionView : MonoBehaviour
    {
        private VisualElement _root;

        private Label _buildVersion;

        private void Awake()
        {
            _root = GetComponent<UIDocument>().rootVisualElement;

            _buildVersion = _root.Q<Label>("build-version");
        }

        private void Start()
        {
            _buildVersion.text = $"[Build: {Application.version}]";
        }
    }
}
