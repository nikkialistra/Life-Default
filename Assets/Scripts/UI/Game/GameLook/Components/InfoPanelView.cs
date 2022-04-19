using System.Collections.Generic;
using Colonists;
using ResourceManagement;
using UI.Game.GameLook.Components.ColonistInfo;
using UnityEngine;
using UnityEngine.UIElements;

namespace UI.Game.GameLook.Components
{
    [RequireComponent(typeof(ColonistInfoView))]
    [RequireComponent(typeof(ColonistsInfoView))]
    [RequireComponent(typeof(EntityInfoView))]
    [RequireComponent(typeof(CommandsView))]
    public class InfoPanelView : MonoBehaviour
    {
        [SerializeField] private VisualTreeAsset _asset;

        private ColonistInfoView _colonistInfoView;
        private ColonistsInfoView _colonistsInfoView;
        private EntityInfoView _entityInfoView;

        private Colonist _shownColonist;
        private Resource _shownResource;
        private ResourceChunk _shownResourceChunk;

        private void Awake()
        {
            _colonistInfoView = GetComponent<ColonistInfoView>();
            _colonistsInfoView = GetComponent<ColonistsInfoView>();
            _entityInfoView = GetComponent<EntityInfoView>();

            Tree = _asset.CloneTree();

            InfoPanel = Tree.Q<VisualElement>("info-panel");
        }

        public VisualElement Tree { get; private set; }
        public VisualElement InfoPanel { get; private set; }

        private void Start()
        {
            HideSelf();
        }

        public void SetColonists(List<Colonist> colonists)
        {
            switch (colonists.Count)
            {
                case 0:
                    if (NotShowingEntityInfo())
                    {
                        HidePanels();
                        HideSelf();
                    }
                    break;
                case 1:
                    SetColonist(colonists[0]);
                    break;
                default:
                    PrepareEmptyPanel();
                    ShowColonistsInfo(colonists.Count);
                    break;
            }
        }

        private bool NotShowingEntityInfo()
        {
            return _shownResource == null && _shownResourceChunk == null;
        }

        public void SetColonist(Colonist colonist)
        {
            PrepareEmptyPanel();
            _shownColonist = colonist;
            ShowColonistInfo();
        }
        
        public void UnsetColonist(Colonist colonist)
        {
            if (_shownColonist == colonist)
            {
                _shownColonist = null;
                _colonistInfoView.HideSelf();
                HideSelf();
            }
        }

        public void SetResource(Resource resource)
        {
            PrepareEmptyPanel();
            _shownResource = resource;
            ShowResourceInfo();
        }

        public void UnsetResource(Resource resource)
        {
            if (_shownResource == resource)
            {
                _shownResource = null;
                _entityInfoView.HideSelf();
                HideSelf();
            }
        }
        
        public void SetResourceChunk(ResourceChunk resourceChunk)
        {
            PrepareEmptyPanel();
            _shownResourceChunk = resourceChunk;
            ShowResourceChunkInfo();
        }

        public void UnsetResourceChunk(ResourceChunk resourceChunk)
        {
            if (_shownResourceChunk == resourceChunk)
            {
                _shownResourceChunk = null;
                _entityInfoView.HideSelf();
                HideSelf();
            }
        }
        
        private void ShowColonistInfo()
        {
            _colonistInfoView.ShowSelf();
            _colonistInfoView.FillIn(_shownColonist);
        }

        private void ShowColonistsInfo(int count)
        {
            _colonistsInfoView.ShowSelf();
            _colonistsInfoView.SetCount(count);
        }
        
        private void ShowResourceInfo()
        {
            _entityInfoView.ShowSelf();
            _entityInfoView.FillIn(_shownResource);
        }
        
        private void ShowResourceChunkInfo()
        {
            _entityInfoView.ShowSelf();
            _entityInfoView.FillIn(_shownResourceChunk);
        }

        private void HideSelf()
        {
            InfoPanel.style.display = DisplayStyle.None;
        }

        private void ShowSelf()
        {
            InfoPanel.style.display = DisplayStyle.Flex;
        }

        private void PrepareEmptyPanel()
        {
            UnsetAll();
            ShowSelf();
            HidePanels();
        }

        private void UnsetAll()
        {
            _shownColonist = null;
            _shownResource = null;
        }

        private void HidePanels()
        {
            _colonistInfoView.HideSelf();
            _colonistsInfoView.HideSelf();
            _entityInfoView.HideSelf();
        }
    }
}
