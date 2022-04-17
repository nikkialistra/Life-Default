using UnityEngine;
using UnityEngine.UIElements;

namespace UI.Game.GameLook.Components
{
    [RequireComponent(typeof(ResourcesView))]
    public class StockView : MonoBehaviour
    {
        [SerializeField] private VisualTreeAsset _asset;
        
        private Button _resources;
        
        private ResourcesView _resourcesView;

        private void Awake()
        {
            _resourcesView = GetComponent<ResourcesView>();
            
            Tree = _asset.CloneTree();

            Content = Tree.Q<VisualElement>("content");

            _resources = Tree.Q<Button>("resources");
        }

        public VisualElement Tree { get; private set; }
        public VisualElement Content { get; private set; }

        private void Start()
        {
            _resourcesView.ShowSelf();
        }

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
