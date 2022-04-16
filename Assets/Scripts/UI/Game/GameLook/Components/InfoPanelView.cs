using System.Collections.Generic;
using Colonists.Colonist;
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
        private const string VisualTreePath = "UI/Markup/GameLook/Components/InfoPanel";

        private ColonistInfoView _colonistInfoView;
        private ColonistsInfoView _colonistsInfoView;
        private EntityInfoView _entityInfoView;

        private ColonistFacade _shownColonist;
        private Resource _shownResource;

        private void Awake()
        {
            _colonistInfoView = GetComponent<ColonistInfoView>();
            _colonistsInfoView = GetComponent<ColonistsInfoView>();
            _entityInfoView = GetComponent<EntityInfoView>();

            Tree = Resources.Load<VisualTreeAsset>(VisualTreePath).CloneTree();

            InfoPanel = Tree.Q<VisualElement>("info-panel");
        }

        public VisualElement Tree { get; private set; }
        public VisualElement InfoPanel { get; private set; }

        private void Start()
        {
            HideSelf();
        }

        public void SetColonists(List<ColonistFacade> colonists)
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

        public void SetColonist(ColonistFacade colonist)
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

        public void UnsetColonistInfo(ColonistFacade colonist)
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

        private void ShowColonistInfo(ColonistFacade colonist)
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
