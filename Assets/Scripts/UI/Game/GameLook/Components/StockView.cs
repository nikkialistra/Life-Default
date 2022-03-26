using UnityEngine;
using UnityEngine.UIElements;

namespace UI.Game.GameLook.Components
{
    [RequireComponent(typeof(ResourcesView))]
    public class StockView : MonoBehaviour
    {
        private const string VisualTreePath = "UI/Markup/GameLook/Components/Stock";
        
        private Button _resources;
        
        private ResourcesView _resourcesView;

        private void Awake()
        {
            _resourcesView = GetComponent<ResourcesView>();
            
            Tree = Resources.Load<VisualTreeAsset>(VisualTreePath).CloneTree();

            Content = Tree.Q<VisualElement>("content");

            _resources = Tree.Q<Button>("resources");
        }

        public VisualElement Tree { get; private set; }
        public VisualElement Content { get; private set; }

        private void OnEnable()
        {
            _resources.clicked += OnResourcesClick;
        }

        private void OnDisable()
        {
            _resources.clicked -= OnResourcesClick;
        }

        private void OnResourcesClick()
        {
            if (_resourcesView.Shown)
            {
                _resourcesView.HideSelf();
            }
            else
            {
                HideAll();
                _resourcesView.ShowSelf();
            }
        }

        private void HideAll()
        {
            _resourcesView.HideSelf();
        }
    }
}
