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
                    if (_shownResource == null)
                    {
                        HidePanels();
                        HideSelf();
                    }
                    break;
                case 1:
                    PrepareEmptyPanel();
                    ShowColonistInfo(colonists[0]);
                    break;
                default:
                    PrepareEmptyPanel();
                    ShowColonistsInfo(colonists.Count);
                    break;
            }
        }

        public void SetColonist(Colonist colonist)
        {
            PrepareEmptyPanel();
            ShowColonistInfo(colonist);
        }

        public void SetResource(Resource resource)
        {
            PrepareEmptyPanel();

            _shownResource = resource;

            ShowResourceInfo(resource);
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

        public void UnsetColonistInfo(Colonist colonist)
        {
            if (_shownColonist == colonist)
            {
                _shownColonist = null;
                _colonistInfoView.HideSelf();
                HideSelf();
            }
        }

        private void ShowResourceInfo(Resource resource)
        {
            _entityInfoView.ShowSelf();
            
            _entityInfoView.FillIn(resource);
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

        private void ShowColonistInfo(Colonist colonist)
        {
            _shownColonist = colonist;
            
            _colonistInfoView.ShowSelf();
            _colonistInfoView.FillIn(colonist);
        }

        private void ShowColonistsInfo(int count)
        {
            _colonistsInfoView.ShowSelf();
            _colonistsInfoView.SetCount(count);
        }

        private void HidePanels()
        {
            _colonistInfoView.HideSelf();
            _colonistsInfoView.HideSelf();
            _entityInfoView.HideSelf();
        }
    }
}
