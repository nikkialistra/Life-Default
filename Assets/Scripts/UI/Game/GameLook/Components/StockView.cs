using UnityEngine;
using UnityEngine.UIElements;

namespace UI.Game.GameLook.Components
{
    [RequireComponent(typeof(ResourcesView))]
    [RequireComponent(typeof(QuestsView))]
    public class StockView : MonoBehaviour
    {
        [SerializeField] private VisualTreeAsset _asset;
        
        private Button _resources;
        private Button _quests;

        private ResourcesView _resourcesView;
        private QuestsView _questsView;

        private void Awake()
        {
            _resourcesView = GetComponent<ResourcesView>();
            _questsView = GetComponent<QuestsView>();
            
            Tree = _asset.CloneTree();

            Content = Tree.Q<VisualElement>("content");

            _resources = Tree.Q<Button>("resources");
            _quests = Tree.Q<Button>("quests");
        }

        public VisualElement Tree { get; private set; }
        public VisualElement Content { get; private set; }

        private void Start()
        {
            _resourcesView.ShowSelf();
        }

        private void OnEnable()
        {
            _resources.clicked += ToggleResources;
            _quests.clicked += ToggleQuests;
        }

        private void OnDisable()
        {
            _resources.clicked -= ToggleResources;
            _quests.clicked -= ToggleQuests;
        }

        private void ToggleResources()
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

        private void ToggleQuests()
        {
            if (_questsView.Shown)
            {
                _questsView.HideSelf();
            }
            else
            {
                HideAll();
                _questsView.ShowSelf();
            }
        }

        private void HideAll()
        {
            _resourcesView.HideSelf();
            _questsView.HideSelf();
        }
    }
}
